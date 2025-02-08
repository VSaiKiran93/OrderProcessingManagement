using System;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.API.Services;
using OrderProcessingSystem.API.Repositories;
using OrderProcessingSystem.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register dependencies with the correct lifetimes
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Scoped lifetime
builder.Services.AddScoped<IOrderService, OrderService>(); // Scoped lifetime

var app = builder.Build();

// Enable Swagger for API documentation (should be before app.Run())
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uncomment if HTTPS redirection is needed
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

// WeatherForecast minimal API (optional, can be removed if not needed)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

// WeatherForecast record (optional, can be removed if not needed)
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}