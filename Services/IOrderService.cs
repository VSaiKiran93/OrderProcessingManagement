using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderProcessingSystem.API.Models;

namespace OrderProcessingSystem.API.Services
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(Order order);
        Task<bool> CancelOrderAsync(int orderId);
        Task FulfillOrdersAsync();
        Task<Order> GetOrderAsync(int id);
    }
}