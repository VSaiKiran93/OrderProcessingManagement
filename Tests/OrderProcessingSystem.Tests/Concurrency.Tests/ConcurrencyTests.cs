using Moq;
using Xunit;
using OrderProcessingSystem.API.Services;
using OrderProcessingSystem.API.Models;
using OrderProcessingSystem.API.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Tests.Concurrency.Tests
{
    public class ConcurrencyTests
    {
        private readonly Mock<IOrderService> _mockOrderService;

        public ConcurrencyTests()
        {
            _mockOrderService = new Mock<IOrderService>();
        }

        [Theory]
        [InlineData(2)] // 2 concurrent orders
        [InlineData(3)] // 3 concurrent orders
        public async Task PlaceOrder_ConcurrentRequests_HandlesInventoryCorrectly(int concurrencyLevel)
        {
            // Arrange
            var setup = _mockOrderService
                .SetupSequence(service => service.PlaceOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(new Order { Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 6 } } })
                .ReturnsAsync(new Order { Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 6 } } })
                .ThrowsAsync(new InsufficientStockException("Insufficient stock"));

            var tasks = new List<Task>();
            var exceptions = new List<Exception>();

            // Act: Simulate concurrent orders
            for (int i = 0; i < concurrencyLevel; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await _mockOrderService.Object.PlaceOrderAsync(new Order
                        {
                            Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 6 } }
                        });
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

            // Verify that PlaceOrderAsync was called exactly concurrencyLevel times
            _mockOrderService.Verify(service => service.PlaceOrderAsync(It.IsAny<Order>()), Times.Exactly(concurrencyLevel));

            // Additional assertions can be added, like verifying if stock is decremented correctly or handling other edge cases
        }
    }
}
