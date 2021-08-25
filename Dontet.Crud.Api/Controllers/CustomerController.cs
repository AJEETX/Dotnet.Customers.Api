using Dotnet.Customers.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerContext _customerContext;

        public CustomerController(CustomerContext customerContext)
        {
            _customerContext = customerContext;
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        public async Task<ActionResult<Customer>> GetById(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var customer = await _customerContext.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
        [HttpGet("{q}")]
        [ProducesResponseType(typeof(IEnumerable<Customer>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest($"The search parameter {nameof(q)} is empty");
            }
            var customers = await _customerContext.Customers.Where(c => c.FirstName.ToLowerInvariant().Trim().StartsWith(q.ToLowerInvariant().Trim())
             || c.LastName.ToLowerInvariant().Trim().StartsWith(q.ToLowerInvariant().Trim())).ToListAsync();
            return Ok(customers);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PutAsync(int id, Domain.Customer customer)
        {
            if (!ModelState.IsValid || id != customer.Id || customer.Id == 0)
            {
                return BadRequest();
            }

            _customerContext.Customers.Update(customer);
            await _customerContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Domain.Customer), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostAsync(Domain.Customer customer)
        {
            if (customer == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            await _customerContext.Customers.AddAsync(customer);
            await _customerContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var customer2Delete = await _customerContext.Customers.FindAsync(id);

            if (customer2Delete == default)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
