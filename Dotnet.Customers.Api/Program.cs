using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dotnet.Customers.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //1. Get Host
            var host = CreateWebHostBuilder(args).Build();

            //2. Find the service layer within our scope.
            using (var scope = host.Services.CreateScope())
            {
                //3. Get the instance of CustomerDBContext in our services layer
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<CustomerContext>();

                //4. Call the SeedCustomerData to create sample data
                SeedCustomerData.Initialize(services);
            }

            //Continue to run the application
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}