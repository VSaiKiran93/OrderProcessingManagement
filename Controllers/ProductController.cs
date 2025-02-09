using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.API.Models;
using OrderProcessingSystem.API.Repositories;

namespace OrderProcessingSystem.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repository.GetAllAsync(); // Use async method
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product != null ? Ok(product) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _repository.AddAsync(product); // Use async method
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var existing = await _repository.GetByIdAsync(id); 
            if (existing == null) return NotFound();
            await _repository.UpdateAsync(product); 
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id); 
            return NoContent();
        }

        [HttpPost("{id}/restock")]
        public async Task<IActionResult> RestockProduct(int id, [FromBody] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.StockQuantity += quantity;
            await _repository.UpdateAsync(product);

            return Ok(product);
        }
    }
}
