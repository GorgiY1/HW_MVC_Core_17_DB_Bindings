using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_app.Models;

//Реалізувати контролери для Customer, Order, OrderProduct
namespace Shop_app.Controllers.API
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public OrdersController(ShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _shopContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _shopContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound("Order not found.");
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _shopContext.Orders.Add(order);
            await _shopContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _shopContext.Entry(order).State = EntityState.Modified;
            await _shopContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _shopContext.Orders.FindAsync(id);
            if (order == null) return NotFound("Order not found.");

            _shopContext.Orders.Remove(order);
            await _shopContext.SaveChangesAsync();
            return NoContent();
        }
    }

}
