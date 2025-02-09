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
builder.Services.AddScoped<IOrderService, OrderService>(); 
builder.Services.AddHostedService<OrderFulfillmentService>(); // Singleton lifetime

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

app.Run();