using Dotnet.Customers.Api.Common;
using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Domain.Services;
using Dotnet.Customers.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotnet.Customers.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwagger>()
            .AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();
            });
            services.AddDbContext<CustomerContext>(opt => opt.UseInMemoryDatabase("TestDb"))
            .AddMemoryCache()
            .AddScoped<ICustomerService, CustomerService>()
            .Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddFeatureManagement();
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider, ILoggerFactory loggerFactory)
        {
            app.UseSwagger().UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(string.Format(SwaggerSettings.Endpoints, description.GroupName), description.GroupName.ToUpperInvariant());
                }
            })
            .AddExceptionHandler(loggerFactory, Configuration)
            .UseMiddleware<RequestResponseLogger>()
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}