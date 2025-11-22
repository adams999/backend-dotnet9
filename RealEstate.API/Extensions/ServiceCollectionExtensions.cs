using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.API.Configuration;
using RealEstate.API.Data;
using RealEstate.API.Services;

namespace RealEstate.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<ITransactionService, TransactionService>();

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind database settings
        var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>() ?? new DatabaseSettings();
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));

        // Add DbContext
        services.AddDbContext<RealEstateDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.CommandTimeout(databaseSettings.CommandTimeout);
                    npgsqlOptions.EnableRetryOnFailure(databaseSettings.MaxRetryCount);
                });
            
            if (databaseSettings.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }

    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        // Add FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }

    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHealthChecks();

        return services;
    }

    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind CORS settings
        var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>() ?? new CorsSettings();
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SectionName));

        services.AddCors(options =>
        {
            options.AddPolicy("ApiCorsPolicy", builder =>
            {
                if (corsSettings.AllowedOrigins.Length > 0)
                {
                    builder.WithOrigins(corsSettings.AllowedOrigins);
                }
                else
                {
                    builder.AllowAnyOrigin();
                }

                if (corsSettings.AllowedMethods.Length > 0)
                {
                    builder.WithMethods(corsSettings.AllowedMethods);
                }
                else
                {
                    builder.AllowAnyMethod();
                }

                if (corsSettings.AllowedHeaders.Length > 0)
                {
                    builder.WithHeaders(corsSettings.AllowedHeaders);
                }
                else
                {
                    builder.AllowAnyHeader();
                }

                if (corsSettings.AllowCredentials)
                {
                    builder.AllowCredentials();
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"),
                new QueryStringApiVersionReader("api-version")
            );
        }).AddMvc();

        return services;
    }

    public static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        return services;
    }
}


