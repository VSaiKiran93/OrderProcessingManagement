using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OrderProcessingSystem.API.Models {

    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        // RowVersion for concurrency control
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}