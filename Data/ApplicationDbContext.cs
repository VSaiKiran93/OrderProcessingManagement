using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.API.Models;

namespace OrderProcessingSystem.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet properties 
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // one-to-many relationship between Order and OrderItem
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);
            
            // many-to-one relationship between OrderItem and Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            // Seed initial data for products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Product 1", Price = 19.99M,  StockQuantity = 100 },
                new Product { Id = 2, Name = "Product 2", Price = 29.99M, StockQuantity = 50 }
            );
        }
    }
}