using AutoMapper;
using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Dtos;

namespace Dotnet.Customers.Api.Common
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ReverseMap();
        }
    }
}