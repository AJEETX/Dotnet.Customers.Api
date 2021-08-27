using Dotnet.Customers.Api.Common;
using Dotnet.Customers.Api.Domain.Models;
using Dotnet.Customers.Api.Domain.Services;
using Dotnet.Customers.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            services
            .AddAutoMapper(typeof(Startup)).AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwagger>()
            .AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();
            })
            .AddDbContext<CustomerContext>(opt => opt.UseInMemoryDatabase("TestDb"))
            .AddMemoryCache()
            .AddScoped<ICustomerService, CustomerService>()
            .Configure<AppSettings>(Configuration.GetSection("AppSettings"))
            .AddFeatureManagement();
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