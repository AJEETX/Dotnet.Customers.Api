using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Common
{
    [ApiVersion(ApiVersionNumbers.V1)]
    [ApiVersion(ApiVersionNumbers.V2)]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route(RouteNames.RouteAttribute)]
    public class BaseApiController : ControllerBase
    {
    }
}
