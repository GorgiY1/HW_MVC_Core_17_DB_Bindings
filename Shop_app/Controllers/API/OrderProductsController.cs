using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_app.Models;

namespace Shop_app.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderProductsController : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public OrderProductsController(ShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderProducts()
        {
            var orderProducts = await _shopContext.OrderProducts
                .Include(op => op.Order)
                .Include(op => op.Product)
                .ToListAsync();
            return Ok(orderProducts);
        }

        [HttpGet("{orderId}/{productId}")]
        public async Task<IActionResult> GetOrderProduct(int orderId, int productId)
        {
            var orderProduct = await _shopContext.OrderProducts
                .Include(op => op.Order)
                .Include(op => op.Product)
                .FirstOrDefaultAsync(op => op.OrderId == orderId && op.ProductId == productId);

            if (orderProduct == null) return NotFound("OrderProduct not found.");
            return Ok(orderProduct);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderProduct([FromBody] OrderProduct orderProduct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _shopContext.OrderProducts.Add(orderProduct);
            await _shopContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderProduct), new { orderId = orderProduct.OrderId, productId = orderProduct.ProductId }, orderProduct);
        }

        [HttpDelete("{orderId}/{productId}")]
        public async Task<IActionResult> DeleteOrderProduct(int orderId, int productId)
        {
            var orderProduct = await _shopContext.OrderProducts
                .FirstOrDefaultAsync(op => op.OrderId == orderId && op.ProductId == productId);

            if (orderProduct == null) return NotFound("OrderProduct not found.");

            _shopContext.OrderProducts.Remove(orderProduct);
            await _shopContext.SaveChangesAsync();
            return NoContent();
        }
    }

}
