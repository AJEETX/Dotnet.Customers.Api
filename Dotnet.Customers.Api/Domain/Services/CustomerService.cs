using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Domain.Services
{
    //THE BEHAVIOUR BE `public'
    public interface ICustomerService
    {
        Task<Customer> GetByIdAsync(int id);

        Task<IList<Customer>> SearchAsync(string q);

        Task<Customer> AddAsync(CustomerDto customerDto);

        Task UpdateAsync(int id, CustomerDto customerDto);

        Task DeleteAsync(int id);
    }

    //THE IMPLMENTATION BETTER BE `internal'
    internal class CustomerService : ICustomerService
    {
        private readonly CustomerContext _customerContext;
        private readonly IMemoryCache _memoryCache;
        private readonly AppSettings _appSettings;
        public CustomerService(IMemoryCache memoryCache, IOptions<AppSettings> options,CustomerContext customerContext)
        {
            _memoryCache = memoryCache;
            _appSettings = options.Value;
            _customerContext = customerContext;
        }

        public async Task<Customer> AddAsync(CustomerDto customerDto)
        {
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR

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
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR

            var customer = await GetByIdAsync(id);
            _customerContext.Customers.Remove(customer);
            await _customerContext.SaveChangesAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR
            if (_memoryCache.TryGetValue(nameof(GetByIdAsync) + id.ToString(), out Customer cachedResponse)) return cachedResponse;

            var customer = await _customerContext.Customers.FindAsync(id);
            SetCache(nameof(GetByIdAsync)+ id.ToString(), customer);
            return customer;
        }

        public async Task<IList<Customer>> SearchAsync(string q)
        {
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR
            if (string.IsNullOrWhiteSpace(q)) return default;

            if (_memoryCache.TryGetValue(nameof(SearchAsync) + q.ToLowerInvariant().Trim(), out IList<Customer> cachedResponse)) return cachedResponse;
            q = q.ToLowerInvariant().Trim();
            
            var customers = await _customerContext.Customers.Where(c =>
            c.FirstName.ToLowerInvariant().Trim().StartsWith(q) || c.LastName.ToLowerInvariant().Trim().StartsWith(q)).ToListAsync();

            SetCache(nameof(SearchAsync) + q, customers);

            return customers;
        }

        public async Task UpdateAsync(int id, CustomerDto customerDto)
        {
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR
            if (0 >= id || customerDto == default) return;

            var customer = await GetByIdAsync(id);

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.DateOfBirth = customerDto.DateOfBirth;

            _customerContext.Customers.Update(customer);

            await _customerContext.SaveChangesAsync();
        }
        private void SetCache(string key, object value)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(_appSettings.CacheSettings.AbsoluteExpiration),
                SlidingExpiration = TimeSpan.FromMinutes(_appSettings.CacheSettings.SlidingExpiration)
            });
        }
    }
}