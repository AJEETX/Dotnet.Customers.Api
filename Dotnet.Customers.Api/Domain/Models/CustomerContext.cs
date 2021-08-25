using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Domain.Models
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(
                b =>
                {
                    b.Property("Id");
                    b.HasKey("Id");
                    b.Property(e => e.FirstName).IsRequired().HasMaxLength(250);
                    b.Property(e => e.LastName).IsRequired().HasMaxLength(250);
                    b.Property(e => e.DateOfBirth);
                });

            modelBuilder.Entity<Customer>()
                .HasIndex(p => p.FirstName);
            modelBuilder.Entity<Customer>()
                .HasIndex(p => p.LastName);
        }
    }
}
