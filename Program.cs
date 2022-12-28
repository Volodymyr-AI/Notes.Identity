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

            

   
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.UseRouting();
            app.UseIdentityServer();
            app.Run();
        }
    }
}