using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.API.Models;
using OrderProcessingSystem.API.Data;

namespace OrderProcessingSystem.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbcontext;

        public ProductRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // Get all products asynchronously
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbcontext.Products.ToListAsync();
        }

        // Get product by ID asynchronously
        public async Task<Product?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid product ID");
            }

            var product = await _dbcontext.Products.FirstOrDefaultAsync(p => p.Id == id);
            return product;
        }

        // Add a new product asynchronously
        public async Task AddAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await _dbcontext.Products.AddAsync(product);
            await _dbcontext.SaveChangesAsync();
        }

        // Update an existing product asynchronously
        public async Task UpdateAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var existing = await _dbcontext.Products.FindAsync(product.Id);
            if (existing == null)
            {
                throw new ArgumentException("Product not found");
            }

            // Update the existing product's properties
            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.StockQuantity = product.StockQuantity;

            _dbcontext.Products.Update(product);
            await _dbcontext.SaveChangesAsync();
        }

        // Delete a product by ID asynchronously
        public async Task DeleteAsync(int id)
        {
            var product = await _dbcontext.Products.FindAsync(id);
            if (product != null)
            {
                _dbcontext.Products.Remove(product);
                await _dbcontext.SaveChangesAsync();
            }
        }

        // Save changes asynchronously (optional, can be removed if not needed)
        public Task SaveAsync()
        {
            return _dbcontext.SaveChangesAsync();
        }
    }
}