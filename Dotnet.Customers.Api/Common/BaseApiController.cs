using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

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