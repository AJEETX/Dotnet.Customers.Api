using AutoMapper;
using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Infrastructure
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
