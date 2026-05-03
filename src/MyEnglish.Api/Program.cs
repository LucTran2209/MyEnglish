
using FastEndpoints;
using FastEndpoints.Swagger;
using MyEnglish.Api.Middleware;
using MyEnglish.Application.DependencyInjections;
using MyEnglish.Infrastructure.DependencyInjections;
using MyEnglish.Persistence.DependencyInjections;

namespace MyEnglish.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        app.Run();
    }
}
