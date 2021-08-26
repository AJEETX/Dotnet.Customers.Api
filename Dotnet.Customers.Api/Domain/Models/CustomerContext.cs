using Microsoft.EntityFrameworkCore;

namespace Dotnet.Customers.Api.Domain.Models
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
    }
}