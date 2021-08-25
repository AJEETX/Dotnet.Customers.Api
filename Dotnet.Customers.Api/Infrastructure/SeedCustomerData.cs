using Dotnet.Customers.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Dotnet.Customers.Api.Infrastructure
{
    public class SeedCustomerData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new CustomerContext(
            serviceProvider.GetRequiredService<DbContextOptions<CustomerContext>>()))
            {
                if (!dbContext.Customers.Any())
                    dbContext.Customers.AddRange(new[]
                    {
                    new Customer(){
                        FirstName = "Azy",
                        LastName = "kumar",
                        DateOfBirth = DateTime.Now.AddYears(-19)
                    },
                    new Customer(){
                        FirstName = "Aby",
                        LastName = "Baby",
                        DateOfBirth = DateTime.Now.AddYears(-17)
                    }
                });

                dbContext.SaveChanges();
            }
        }
    }
}