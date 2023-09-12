using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Model.Context
{
    public class MySqlContext : DbContext
    {
        public DbSet<EmailLog> Logs { get; set; }

        public MySqlContext()
        {
        }

        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)
        {
        }
    }
}
