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
            // FLAW 1: Synchronous Database Call
            // This blocks the thread while waiting for the database, severely limiting how many 
            // concurrent requests the API can handle.
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // FLAW 2: The "ToList()" Memory Bomb
            // By calling .ToList() BEFORE .Where(), EF Core fetches the ENTIRE Orders table 
            // from SQL Server into the web server's RAM, and then filters it in memory.
            var userOrders = _context.Orders.ToList().Where(o => o.UserId == user.Id);

            var response = new
            {
                FullName = user.Name,
                ContactEmail = user.Email,

                // FLAW 3: Inefficient Projection
                // This forces the app to do the string formatting in memory rather than 
                // letting the database handle the data shaping.
                OrderSummaries = userOrders.Select(o => $"Order #{o.Id} - RM{o.Total}")
            };

            return Ok(response);
        }
    }
}
