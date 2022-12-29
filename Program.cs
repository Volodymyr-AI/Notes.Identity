using Microsoft.EntityFrameworkCore;
using Notes.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Notes.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.NetworkInformation;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Notes.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DbConnextion");
            builder.Services.AddDbContext<AuthDbContext>(x => x.UseSqlServer(connectionString));

            builder.Services.AddIdentity<AppUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 12;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = true;
                config.Password.RequireLowercase = true;
            })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();


            builder.Services.AddIdentityServer()
                .AddAspNetIdentity<AppUser>()
                .AddInMemoryApiResources(Configuration.ApiResource)
                .AddInMemoryIdentityResources(Configuration.IdentityResources)
                .AddInMemoryApiScopes(Configuration.ApiScopes)
                .AddInMemoryClients(Configuration.Clients)
                .AddDeveloperSigningCredential();

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Notes.Identity.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });

            builder.Services.AddControllersWithViews();

   
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");
            using(var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<AuthDbContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while app initialization");
                }
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "Styles")),
                RequestPath = "/styles"

            });
            app.UseRouting();

            
            app.MapDefaultControllerRoute();

            app.UseIdentityServer();
            app.Run();
        }
    }
}