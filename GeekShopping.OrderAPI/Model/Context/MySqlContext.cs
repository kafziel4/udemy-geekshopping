using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Model.Context
{
    public class MySqlContext : DbContext
    {
        public DbSet<OrderHeader> Headers { get; set; }
        public DbSet<OrderDetail> Details { get; set; }

        public MySqlContext()
        {
        }

        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)
        {
        }
    }
}
