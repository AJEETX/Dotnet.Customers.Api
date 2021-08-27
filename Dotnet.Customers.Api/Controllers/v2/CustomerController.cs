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

namespace Dotnet.Customers.Api.v2.Controllers
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

        [FeatureGate(Features.CUSTOMER_OFF)]
        [ApiVersion(ApiVersionNumbers.V2)]
        [HttpGet("{id:int}", Name = nameof(GetByIdAsync))]
        [RequestRateLimit(Name = nameof(GetByIdAsync), Order = 1, Seconds = 1)]
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

        [FeatureGate(Features.CUSTOMER_OFF)]
        [ApiVersion(ApiVersionNumbers.V2)]
        [HttpGet("{q}")]
        [RequestRateLimit(Name = nameof(SearchAsync), Order = 1, Seconds = 1)]
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
    }
}