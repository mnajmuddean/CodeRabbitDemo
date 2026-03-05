using Microsoft.EntityFrameworkCore;

namespace CodeRabbitDemo.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new();
    }

    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Najmuddin", Email = "najmuddin@example.com" }
            );
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 101, UserId = 1, Total = 50.00m },
                new Order { Id = 102, UserId = 1, Total = 120.50m }
            );
        }
    }
}