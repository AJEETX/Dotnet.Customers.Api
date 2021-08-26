using Dotnet.Customers.Api.Domain.Services;
using Dotnet.Customers.Api.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FeatureGate("customer")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerContext;

        public CustomerController(ICustomerService customerContext)
        {
            _customerContext = customerContext;
        }

        [HttpGet("{id:int}", Name = nameof(GetByIdAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0) return BadRequest($"The {nameof(id)} is invalid");

            var customer = await _customerContext.GetByIdAsync(id);

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

            var customers = await _customerContext.SearchAsync(q);

            return Ok(customers);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync(CustomerDto customerDto)
        {
            if (customerDto == null || !ModelState.IsValid) return BadRequest($"The {nameof(customerDto)} is invalid");

            var customer = await _customerContext.AddAsync(customerDto);

            return CreatedAtRoute(nameof(GetByIdAsync), new { id = customer.Id }, customerDto);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutAsync(int id, CustomerDto customerDto)
        {
            if (!ModelState.IsValid || id <= 0 || customerDto == default) return BadRequest($"The {nameof(id)} and/or {nameof(customerDto)} is invalid");

            var customer = await _customerContext.GetByIdAsync(id);

            if (customer == default) return NotFound($"Oops !!! Customer not found with {nameof(id)}: {id}");

            await _customerContext.UpdateAsync(id, customerDto);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0) return BadRequest($"The {nameof(id)} is invalid");

            var customer = await _customerContext.GetByIdAsync(id);

            if (customer == default) return NotFound($"Oops !!! Customer not found with {nameof(id)}: {id}");

            await _customerContext.DeleteAsync(id);
            return NoContent();
        }
    }
}