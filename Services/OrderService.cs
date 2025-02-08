using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderProcessingSystem.API.Models;
using OrderProcessingSystem.API.Repositories;
using OrderProcessingSystem.API.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderProcessingSystem.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;

        public OrderService(ApplicationDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public async Task<Order> PlaceOrderAsync(Order order)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate and update stock for each item in the order
                    foreach (var item in order.Items)
                    {
                        var product = await _productRepository.GetByIdAsync(item.ProductId);
                        if (product == null || product.StockQuantity < item.Quantity)
                        {
                            throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}");
                        }

                        // Reduce stock quantity
                        product.StockQuantity -= item.Quantity;
                        _context.Products.Update(product);
                    }

                    // Add the order to the database
                    order.Status = "Pending Fulfillment";
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return order;
                }
                catch (Exception)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id) ?? throw new InvalidOperationException("Order not found");
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var order = await _context.Orders
                        .Include(o => o.Items)
                        .FirstOrDefaultAsync(o => o.Id == orderId);

                    if (order == null || order.Status == "Fulfilled")
                    {
                        return false;
                    }

                    // Restore stock for each item in the order
                    foreach (var item in order.Items)
                    {
                        var product = await _productRepository.GetByIdAsync(item.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity += item.Quantity;
                            _context.Products.Update(product);
                        }
                    }

                    // Update order status
                    order.Status = "Cancelled";
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task FulfillOrdersAsync()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == "Pending Fulfillment")
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Status = "Fulfilled";
                _context.Orders.Update(order);
                Console.WriteLine($"Order {order.Id} fulfilled!");
            }

            await _context.SaveChangesAsync();
        }
    }
}