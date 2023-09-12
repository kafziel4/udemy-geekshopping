using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Model.Context
{
    public class MySqlContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }

        public MySqlContext()
        {
        }

        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)
        {
        }
    }
}
