using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Domain.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetByIdAsync(int id);
        Task<IList<Customer>> SearchAsync(string q);
        Task<Customer> AddAsync(CustomerDto customerDto);
        Task UpdateAsync(int id, CustomerDto customerDto);
        Task DeleteAsync(int id);
    }
    public class CustomerService: ICustomerService
    {
        private readonly CustomerContext _customerContext;

        public CustomerService(CustomerContext customerContext)
        {
            _customerContext = customerContext;
        }

        public async Task<Customer> AddAsync(CustomerDto customerDto)
        {
            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                DateOfBirth = customerDto.DateOfBirth
            };
            await _customerContext.Customers.AddAsync(customer);

            await _customerContext.SaveChangesAsync();
            return customer;
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);
            _customerContext.Customers.Remove(customer);
            await _customerContext.SaveChangesAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _customerContext.Customers.FindAsync(id);
        }

        public async Task<IList<Customer>> SearchAsync(string q)
        {
            var customers = await _customerContext.Customers.Where(c => 
            c.FirstName.ToLowerInvariant().Trim().StartsWith(q.ToLowerInvariant().Trim()) || 
            c.LastName.ToLowerInvariant().Trim().StartsWith(q.ToLowerInvariant().Trim())).ToListAsync();
            return customers;
        }

        public async Task UpdateAsync(int id, CustomerDto customerDto)
        {
            var customer =await GetByIdAsync(id);

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.DateOfBirth = customerDto.DateOfBirth;

            _customerContext.Customers.Update(customer);

            await _customerContext.SaveChangesAsync();
        }
    }
}
