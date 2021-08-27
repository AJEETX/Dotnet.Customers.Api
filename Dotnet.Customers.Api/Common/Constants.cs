namespace Dotnet.Customers.Api.Common
{
    public class SwaggerSettings
    {
        public const string Endpoints = "/swagger/{0}/swagger.json";
        public const string Title = "Dotnet.Customer.Api";
        public const string ApiDescription = " REST API v{0} has been implemented.";
        public const string ApiVersionDescription = "Now, this API version [v{0}] has been deprecated.";
        public const string ContactName = "ajeet kumar";
        public const string ContactEmail = "ajeetkumar@email.com";
        public const string ContactUrl = "https://github.com/ajeetx";
    }

    public static class RouteNames
    {
        public const string RouteAttribute = "api/v{version:apiVersion}/[controller]";
    }

    public class Features
    {
        public const string CUSTOMER_OFF = "CUSTOMER_OFF";
        public const string CUSTOMER_ON = "CUSTOMER_ON";
    }

    public class ApiVersionNumbers
    {
        public const string V1 = "1.0";
        public const string V2 = "2.0";
    }
}