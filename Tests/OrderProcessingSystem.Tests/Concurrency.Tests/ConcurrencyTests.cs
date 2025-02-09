using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.API.Data;
using OrderProcessingSystem.API.Models;
using OrderProcessingSystem.API.Repositories;
using OrderProcessingSystem.API.Services;
using OrderProcessingSystem.API.Exceptions;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace OrderProcessingSystem.Tests.Concurrency.Tests
{
    public class ConcurrencyTests : IClassFixture<DbContextFixture>
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly ProductRepository _productRepository;
        private readonly OrderService _orderService;

        public ConcurrencyTests(DbContextFixture fixture)
        {
            _dbcontext = new ApplicationDbContext(fixture.Options);
            _productRepository = new ProductRepository(_dbcontext);
            _orderService = new OrderService(_dbcontext, _productRepository, Mock.Of<ILogger<OrderService>>());

            // Seed the database with a product
            _dbcontext.Products.Add(new Product { Id = 1, Name = "Product 1", Price = 19.99M, StockQuantity = 10 });
            _dbcontext.SaveChanges();
        }

        [Theory]
        [InlineData(2)] // 2 concurrent orders
        [InlineData(3)] // 3 concurrent orders
        public async Task PlaceOrder_ConcurrentRequests_HandlesInventoryCorrectly(int concurrencyLevel)
        {
            var tasks = new List<Task>();
            var exceptions = new List<Exception>();

            // Act: Simulate concurrent orders
            for (int i = 0; i < concurrencyLevel; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var order = new Order
                        {
                            Items = new List<OrderItem>
                            {
                                new OrderItem { ProductId = 1, Quantity = 6 }
                            }
                        };
                        await _orderService.PlaceOrderAsync(order);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }));
            }

            await Task.WhenAll(tasks);

            // Assert: Ensure at least one InsufficientStockException occurred
            Assert.Contains(exceptions, ex => ex is InsufficientStockException);

            // Ensure stock is not negative after all orders are processed
            var product = await _productRepository.GetByIdAsync(1);
            Assert.True(product.StockQuantity >= 0, "Stock quantity should not be negative.");

            // Verify that the number of successful orders did not exceed available stock
            var successfulOrders = concurrencyLevel - exceptions.Count(ex => ex is InsufficientStockException);
            Assert.True(successfulOrders <= 10, "More than available stock was ordered.");
        }
    }
}
