
using FastEndpoints;
using FastEndpoints.Swagger;
using MyEnglish.Api.Configuration;
using MyEnglish.Api.Middleware;
using MyEnglish.Application.DependencyInjections;
using MyEnglish.Infrastructure.DependencyInjections;
using MyEnglish.Persistence.DependencyInjections;
using Serilog;

namespace MyEnglish.Api;

public class Program
{
    public static void Main(string[] args)
    {
        // Configure Serilog
        SerilogConfiguration.ConfigureSerilog();

        try
        {
            Log.Information("Starting MyEnglish API application");

            var builder = WebApplication.CreateBuilder(args);

            // Add Serilog
            builder.Host.UseSerilog();

            // Add layer services
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddPersistence(builder.Configuration);

            // Add FastEndpoints
            builder.Services.AddFastEndpoints();

            // Add Swagger with FastEndpoints
            builder.Services.SwaggerDocument(o =>
            {
                o.DocumentSettings = s =>
                {
                    s.Title = "MyEnglish API";
                    s.Version = "v1";
                    s.Description = "A comprehensive English learning platform API built with DDD, CQRS, and FastEndpoints";
                };
                o.EnableJWTBearerAuth = true;
            });

            // Add MVC and Health Checks
            builder.Services.AddMvc();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerGen();
            }

            // Add Serilog request logging
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                    diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
                };
            });

            // Add global exception handling
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();

            // Add authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Use FastEndpoints
            app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Serializer.Options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            Log.Information("MyEnglish API application started successfully");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
