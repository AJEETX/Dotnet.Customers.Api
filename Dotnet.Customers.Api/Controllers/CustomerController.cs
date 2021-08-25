using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Controllers
{
    /// <summary>
    /// Customer Web Api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        //inject the DBContext and logger into the controller...
        private readonly CustomerContext _customerContext;

        public CustomerController(CustomerContext customerContext)
        {
            _customerContext = customerContext;
        }
        [HttpGet("{id:int}",Name = "GetByIdAsync")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0) return BadRequest($"The {nameof(id)} is invalid");

            var customer = await _customerContext.Customers.FindAsync(id);

            if (customer == default) return NotFound($"Oops !!! Customer not found with {nameof(id)}: {id}");

            return Ok(customer);
        }
        [HttpGet("{q}")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrEmpty(q)) return BadRequest($"The search parameter {nameof(q)} is empty");
            
            var customers = await _customerContext.Customers.Where(c => c.FirstName.ToLowerInvariant().Trim().StartsWith(q.ToLowerInvariant().Trim())
             || c.LastName.ToLowerInvariant().Trim().StartsWith(q.ToLowerInvariant().Trim())).ToListAsync();
            return Ok(customers);
        }
        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync(CustomerDto customerDto)
        {
            if (customerDto == null || !ModelState.IsValid) return BadRequest($"The {nameof(customerDto)} is invalid");

            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                DateOfBirth = customerDto.DateOfBirth
            };

            await _customerContext.Customers.AddAsync(customer);

            await _customerContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetByIdAsync), new { id = customer.Id }, customerDto);
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType( StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutAsync(int id, CustomerDto customerDto)
        {
            if (!ModelState.IsValid || id <= 0 || customerDto == default) return BadRequest($"The {nameof(id)} and/or {nameof(customerDto)} is invalid");
            
            var customer = await _customerContext.Customers.FindAsync(id);

            if (customer == default)  return NotFound($"Customer with {nameof(id)} : {id} not found");

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.DateOfBirth = customerDto.DateOfBirth;

            _customerContext.Customers.Update(customer);

            await _customerContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType( StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0) return BadRequest($"The {nameof(id)} is invalid");
            

            var customer = await _customerContext.Customers.FindAsync(id);

            if (customer == default)
            {
                return NotFound($"Oops !!! Customer not found with {nameof(id)}: {id}");
            }
            _customerContext.Customers.Remove(customer);
            await _customerContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
