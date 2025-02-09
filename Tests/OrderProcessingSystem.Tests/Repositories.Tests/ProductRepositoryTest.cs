using Moq;
using Xunit;
using OrderProcessingSystem.API.Repositories;
using OrderProcessingSystem.API.Models;
using System;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Tests.Repositories.Tests
{
    public class ProductRepositoryTests
    {
        private readonly Mock<IProductRepository> _mockRepo;

        public ProductRepositoryTests()
        {
            _mockRepo = new Mock<IProductRepository>();
        }

        [Theory]
        [InlineData(1)] // Valid ID
        [InlineData(0)] // Invalid ID (zero)
        [InlineData(-1)] // Invalid ID (negative)
        public async Task GetProduct_ValidId_ReturnsProduct(int id)
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Laptop", Price = 999.99m, StockQuantity = 10 };

            // Setup mock repository based on the input ID
            if (id == 1)
            {
                _mockRepo.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(product);
            }
            else
            {
                _mockRepo.Setup(repo => repo.GetByIdAsync(id)).ThrowsAsync(new ArgumentException("Invalid product ID"));
            }

            // Act & Assert
            if (id <= 0)
            {
                // Test for invalid ID
                await Assert.ThrowsAsync<ArgumentException>(() => _mockRepo.Object.GetByIdAsync(id));
            }
            else
            {
                // Test for valid ID
                var result = await _mockRepo.Object.GetByIdAsync(id);
                Assert.NotNull(result);
                Assert.Equal("Laptop", result.Name);
            }
        }
    }
}
