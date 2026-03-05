using Microsoft.AspNetCore.Mvc;
using CodeRabbitDemo.Data;
using System.Linq;

namespace CodeRabbitDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/orders")]
        public IActionResult GetUserWithOrders(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }


            var userOrders = _context.Orders.ToList().Where(o => o.UserId == user.Id);

            var response = new
            {
                FullName = user.Name,
                ContactEmail = user.Email,

                OrderSummaries = userOrders.Select(o => $"Order #{o.Id} - RM{o.Total}")
            };

            return Ok(response);
        }

        [HttpPost("{id}/apply-discount")]
        public IActionResult ApplyDiscount(int id, [FromQuery] decimal discountAmount)
        {
            var orders = _context.Orders.Where(o => o.UserId == id).ToList();

            foreach (var order in orders)
            {
                order.Total -= discountAmount;

                _context.SaveChanges();
            }

            return Ok($"{orders.Count} discounts applied.");
        }

        [HttpDelete("{id}/delete-orders")]
        public IActionResult DeleteUserOrders(int id)
        {
            try
            {
                var orders = _context.Orders.Where(o => o.UserId == id);
                _context.Orders.RemoveRange(orders);
                _context.SaveChanges();

                return Ok("Orders deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message, Trace = ex.StackTrace });
            }
        }
    }
}
