using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_app.Models;

namespace Shop_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public CustomersController(ShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _shopContext.Customers.Include(c => c.Orders).ToListAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _shopContext.Customers.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound("Customer not found.");
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _shopContext.Customers.Add(customer);
            await _shopContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _shopContext.Entry(customer).State = EntityState.Modified;
            await _shopContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _shopContext.Customers.FindAsync(id);
            if (customer == null) return NotFound("Customer not found.");

            _shopContext.Customers.Remove(customer);
            await _shopContext.SaveChangesAsync();
            return NoContent();
        }
    }

}
