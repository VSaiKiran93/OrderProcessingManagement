using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OrderProcessingSystem.API.Models
{
    public class Order
    {   
        public int Id { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public string Status { get; set; } = "Pending Fulfillment";
    }
    public class OrderItem
    {
        public int Id { get; set; } 
        public int ProductId { get; set; }
         public Product? Product { get; set; } 
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }

    public class OrderRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}