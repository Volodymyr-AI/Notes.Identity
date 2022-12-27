using Microsoft.EntityFrameworkCore;
using Notes.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Notes.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentityServer()
                .AddInMemoryApiResources(Configuration.ApiResource)
                .AddInMemoryIdentityResources(Configuration.IdentityResources)
                .AddInMemoryApiScopes(Configuration.ApiScopes)
                .AddInMemoryClients(Configuration.Clients)
                .AddDeveloperSigningCredential();

            var connectionString = builder.Services.AddDbContext<DbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
            


            var host = Host.CreateDefaultBuilder(args).Build();
            using(var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<AuthDbContext>;
                    DbInitializer.Initialize(context);
                }
                catch(Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>;
                    logger.LogError(ex, "An error occured while app initialization");
                }
            }
            host.Run();


            builder.Services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
                   
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.UseRouting();
            app.UseIdentityServer();
            app.Run();
        }
    }
}