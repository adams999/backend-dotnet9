using RealEstate.API.Configuration;
using RealEstate.API.Data;
using RealEstate.API.Extensions;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting RealEstate API application");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddDatabaseServices(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddValidationServices();
    builder.Services.AddHealthCheckServices();
    builder.Services.AddCorsServices(builder.Configuration);
    builder.Services.AddApiVersioningServices();
    builder.Services.AddMediatRServices();

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    app.UseCustomMiddleware();

    // Add Serilog request logging
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // Initialize database
    app.UseDatabaseInitialization();

    app.UseHttpsRedirection();
    app.UseCors("ApiCorsPolicy");
    app.UseAuthorization();

    // Map endpoints
    app.MapControllers();
    app.UseHealthCheckEndpoints();

    Log.Information("RealEstate API application started successfully");
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

// Make the implicit Program class public for testing
public partial class Program { }
