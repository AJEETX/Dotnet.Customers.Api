using Dotnet.Customers.Api.Common;
using Dotnet.Customers.Api.Domain.Models;
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

        Task<Customer> AddAsync(Customer customer);

        Task UpdateAsync(int id, Customer customer);

        Task DeleteAsync(int id);
    }

    //THE IMPLMENTATION BETTER BE `internal'
    internal class CustomerService : ICustomerService
    {
        private readonly CustomerContext _customerContext;
        private readonly IMemoryCache _memoryCache;
        private readonly AppSettings _appSettings;

        public CustomerService(IMemoryCache memoryCache, IOptions<AppSettings> options, CustomerContext customerContext)
        {
            _memoryCache = memoryCache;
            _appSettings = options.Value;
            _customerContext = customerContext;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR
            if (0 >= id)
            {
                throw new ArgumentOutOfRangeException($"{nameof(CustomerService)}:{nameof(GetByIdAsync)} {nameof(id)} = {id}");
            }

            if (_memoryCache.TryGetValue(nameof(GetByIdAsync) + id.ToString(), out Customer cachedResponse)) return cachedResponse;

            var customer = await _customerContext.Customers.FindAsync(id);

            // SET THE CACHE WITH SEARCH KEYWORD AS KEY AND `customer` as RESPONSE
            SetCache(nameof(GetByIdAsync) + id.ToString(), customer);

            return customer;
        }

        public async Task<IList<Customer>> SearchAsync(string q)
        {
            q = q?.ToLowerInvariant().Trim();

            if (_memoryCache.TryGetValue(nameof(SearchAsync) + q ?? Constants.ALL, out IList<Customer> cachedResponse)) return cachedResponse;

            var customers = await _customerContext.Customers.Where(c =>
            c.FirstName.ToLowerInvariant().Trim().StartsWith(q) || c.LastName.ToLowerInvariant().Trim().StartsWith(q)).ToListAsync();

            // SET THE CACHE WITH SEARCH KEYWORD AS KEY AND `customers` as RESPONSE
            SetCache(nameof(SearchAsync) + q, customers);

            return customers;
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR
            if (default == customer)
            {
                throw new ArgumentNullException($"{nameof(CustomerService)}:{nameof(AddAsync)} {nameof(customer)} :{customer}");
            }

            await _customerContext.Customers.AddAsync(customer);

            await _customerContext.SaveChangesAsync();

            return customer;
        }

        public async Task DeleteAsync(int id)
        {
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR
            if (0 >= id)
            {
                throw new ArgumentOutOfRangeException($"{nameof(CustomerService)}:{nameof(DeleteAsync)} {nameof(id)} = {id}");
            }

            var customer = await GetByIdAsync(id);

            _customerContext.Customers.Remove(customer);

            await _customerContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, Customer customer)
        {
            //ALWAYS GOOD TO INSPECT THE INCOMING TRAFFIC FOR ANY OPEN BEHAVIOUR
            if (0 >= id || customer == default)
            {
                throw new ArgumentOutOfRangeException($"{nameof(CustomerService)}:{nameof(UpdateAsync)} {nameof(id)} = {id} / or {nameof(customer)} is null");
            }

            _customerContext.Customers.Update(customer);

            await _customerContext.SaveChangesAsync();
        }

        #region PRIVATE BEHAVIOUR

        private void SetCache(string key, object value)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(_appSettings.CacheSettings.AbsoluteExpiration),
                SlidingExpiration = TimeSpan.FromMinutes(_appSettings.CacheSettings.SlidingExpiration)
            });
        }

        #endregion PRIVATE BEHAVIOUR
    }
}