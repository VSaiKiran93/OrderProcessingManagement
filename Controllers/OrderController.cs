using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.API.Models;
using OrderProcessingSystem.API.Services;

namespace OrderProcessingSystem.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        // POST: api/orders/place-order
        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest orderRequest)
        {
            try
            {
                // Validate inputs
                if (orderRequest.ProductId <= 0 || orderRequest.Quantity <= 0)
                {
                    return BadRequest("Invalid product ID or quantity.");
                }

                // Convert OrderRequest to Order
                var order = new Order
                {
                    Status = "Pending Fulfillment",
                    Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = orderRequest.ProductId, Quantity = orderRequest.Quantity }
                }
                };

                var createdOrder = await _service.PlaceOrderAsync(order);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var order = await _service.GetOrderAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/orders/5/cancel
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _service.CancelOrderAsync(id);
                return result ? Ok() : BadRequest("Cannot cancel order");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
