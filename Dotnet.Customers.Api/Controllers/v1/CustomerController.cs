using AutoMapper;
using Dotnet.Customers.Api.Common;
using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Domain.Services;
using Dotnet.Customers.Api.Dtos;
using Dotnet.Customers.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.v1.Controllers
{
    public class CustomerController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;

        public CustomerController(IMapper mapper, ICustomerService customerService)
        {
            _mapper = mapper;
            _customerService = customerService;
        }

        [FeatureGate(Features.CUSTOMER_ON)]
        [ApiVersion(ApiVersionNumbers.V1)]
        [RequestRateLimit(Name = nameof(GetByIdAsync), Order = 1, Seconds = 1)]
        [HttpGet("{id:int}", Name = nameof(GetByIdAsync))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"The {nameof(id)} is invalid");
            }

            var customer = await _customerService.GetByIdAsync(id);

            if (customer == default) return NotFound($"Oops !!! Customer not found with {nameof(id)}: {id}");

            var customerDto = _mapper.Map<CustomerDto>(customer);

            return Ok(customerDto);
        }

        [FeatureGate(Features.CUSTOMER_ON)]
        [ApiVersion(ApiVersionNumbers.V1)]
        [HttpGet("{q}")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchAsync(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest($"The search parameter {nameof(q)} is empty");
            }

            var customers = await _customerService.SearchAsync(q);

            var customerDtos = _mapper.Map<IList<CustomerDto>>(customers);

            return Ok(customerDtos);
        }

        [FeatureGate(Features.CUSTOMER_ON)]
        [ApiVersion(ApiVersionNumbers.V1)]
        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync(CustomerDto customerDto)
        {
            if (customerDto == null || !ModelState.IsValid)
            {
                return BadRequest($"The {nameof(customerDto)} is invalid");
            }

            var customer = _mapper.Map<Customer>(customerDto);

            customer = await _customerService.AddAsync(customer);

            return CreatedAtRoute(nameof(GetByIdAsync), new { id = customer.Id }, customerDto);
        }

        [FeatureGate(Features.CUSTOMER_ON)]
        [ApiVersion(ApiVersionNumbers.V1)]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutAsync(int id, CustomerDto customerDto)
        {
            if (!ModelState.IsValid || id <= 0 || customerDto == default)
            {
                return BadRequest($"The {nameof(id)} and/or {nameof(customerDto)} is invalid");
            }

            var customer = await _customerService.GetByIdAsync(id);

            if (customer == default)
            {
                return NotFound($"Oops !!! Customer not found with {nameof(id)}: {id}");
            }

            await _customerService.UpdateAsync(id, customer);

            return NoContent();
        }

        [FeatureGate(Features.CUSTOMER_ON)]
        [ApiVersion(ApiVersionNumbers.V1)]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"The {nameof(id)} is invalid");
            }

            var customer = await _customerService.GetByIdAsync(id);

            if (customer == default)
            {
                return NotFound($"Oops !!! Customer not found with {nameof(id)}: {id}");
            }

            await _customerService.DeleteAsync(id);

            return NoContent();
        }
    }
}