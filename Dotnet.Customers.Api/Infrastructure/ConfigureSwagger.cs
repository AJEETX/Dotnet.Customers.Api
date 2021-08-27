using Dotnet.Customers.Api.Common;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.Customers.Api.Infrastructure
{
    internal class ConfigureSwagger : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwagger(IApiVersionDescriptionProvider provider) => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = SwaggerSettings.Title,
                Contact = new OpenApiContact { Name = SwaggerSettings.ContactName, Email = SwaggerSettings.ContactEmail, Url = new Uri(SwaggerSettings.ContactUrl) },
                Description = string.Format(SwaggerSettings.ApiDescription, description.ApiVersion.ToString()),
                Version = description.ApiVersion.ToString()
            };

            if (description.IsDeprecated)
            {
                info.Description += SwaggerSettings.ApiVersionDescription;
            }

            return info;
        }
    }
}
