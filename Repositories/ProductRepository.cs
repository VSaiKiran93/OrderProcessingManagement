using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderProcessingSystem.API.Models;

namespace OrderProcessingSystem.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 1000, StockQuantity = 10 },
            new Product { Id = 2, Name = "Smartphone", Price = 500, StockQuantity = 20 }
        };

        public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult(_products.AsEnumerable());

        public Task<Product> GetByIdAsync(int id) 
        {
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null) throw new InvalidOperationException("Product not found");
                return Task.FromResult(product);
            }
        }

        public Task AddAsync(Product product)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Price = product.Price;
                existing.StockQuantity = product.StockQuantity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            _products.RemoveAll(p => p.Id == id);
            return Task.CompletedTask;
        }

        public Task SaveAsync() => Task.CompletedTask; // No-op for in-memory implementation
    }
}