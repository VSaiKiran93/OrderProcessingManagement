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
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all products asynchronously
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        // Get product by ID asynchronously
        public async Task<Product?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid product ID");
            }

            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        // Add a new product asynchronously
        public async Task AddAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // Update an existing product asynchronously
        public async Task UpdateAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null)
            {
                throw new ArgumentException("Product not found");
            }

            // Update the existing product's properties
            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.StockQuantity = product.StockQuantity;

            _context.Products.Update(existing);
            await _context.SaveChangesAsync();
        }

        // Delete a product by ID asynchronously
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // Save changes asynchronously (optional, can be removed if not needed)
        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}