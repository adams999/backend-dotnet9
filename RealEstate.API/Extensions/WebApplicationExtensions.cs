using RealEstate.API.Data;
using RealEstate.API.Middleware;

namespace RealEstate.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCustomMiddleware(this WebApplication app)
    {
        // Add exception handling middleware
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        // Add request/response logging middleware (only in development)
        if (app.Environment.IsDevelopment())
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }

        return app;
    }

    public static WebApplication UseDatabaseInitialization(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<RealEstateDbContext>();
            context.Database.EnsureCreated();
            DbInitializer.Seed(context);
        }

        return app;
    }

    public static WebApplication UseHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");

        return app;
    }
}

