using RealEstate.API.Data;
using RealEstate.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddValidationServices();
builder.Services.AddHealthCheckServices();
builder.Services.AddCorsServices();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCustomMiddleware();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Initialize database
app.UseDatabaseInitialization();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.UseHealthCheckEndpoints();

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }
