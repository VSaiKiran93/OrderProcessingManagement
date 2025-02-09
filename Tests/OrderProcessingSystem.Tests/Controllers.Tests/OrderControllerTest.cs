using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.API.Controllers;
using OrderProcessingSystem.API.Services;
using OrderProcessingSystem.API.Models;
using System.Collections.Generic;

namespace OrderProcessingSystem.Tests.Controllers.Tests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrderController _orderController;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _orderController = new OrderController(_mockOrderService.Object);
        }

        [Theory]
        [InlineData(1, 2)] // Valid input
        [InlineData(0, 2)] // Invalid product ID
        [InlineData(1, -2)] // Negative quantity
        public async Task PlaceOrder_ValidInput_ReturnsOk(int productId, int quantity)
        {
            // Arrange
            var orderRequest = new OrderRequest { ProductId = productId, Quantity = quantity };
            
            // Convert OrderRequest to Order
            var mockOrder = new Order
            {
                Id = 1,
                Status = "Pending Fulfillment",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = productId, Quantity = quantity }
                }
            };

            _mockOrderService.Setup(service => service.PlaceOrderAsync(It.IsAny<Order>()))
                            .ReturnsAsync(mockOrder);

            // Act
            var result = await _orderController.PlaceOrder(orderRequest); 

            // Assert
            if (productId <= 0 || quantity <= 0)
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
            else
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
                Assert.NotNull(createdAtActionResult.Value);
                Assert.IsType<Order>(createdAtActionResult.Value);
            }   
        }
    }
}
