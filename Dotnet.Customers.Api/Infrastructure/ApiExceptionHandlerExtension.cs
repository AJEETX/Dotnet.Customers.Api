using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mime;

namespace Dotnet.Customers.Api.Infrastructure
{
    public static class ApiExceptionHandlerExtension
    {
        public static IApplicationBuilder AddExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    //IF ANY EXCEPTION THEN REPORT AND LOG
                    if (contextFeature != null)
                    {
                        //TECHNICAL EXCEPTION FOR TROUBLE-SHOOTING
                        var logger = loggerFactory.CreateLogger(nameof(ApiExceptionHandlerExtension));
                        logger.LogError(contextFeature.Error.Message);

                        //BUSINESS EXCEPTION - EXIT GRACEFULLY
                        await context.Response.WriteAsJsonAsync(new
                        {
                            context.Response.StatusCode,
                            Message = "Oops !!! The error has been notified. Pls try after some time."
                        }.ToString());
                    }
                });
            });
            return app;
        }
    }
}