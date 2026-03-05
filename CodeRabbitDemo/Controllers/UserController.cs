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
    }
}
