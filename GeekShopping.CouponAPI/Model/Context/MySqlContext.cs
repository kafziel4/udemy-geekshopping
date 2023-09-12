using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Model.Context
{
    public class MySqlContext : DbContext
    {
        public DbSet<Coupon> Coupons { get; set; }

        public MySqlContext()
        {
        }

        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var coupons = new List<Coupon>()
            {
                new Coupon
                {
                    Id = 1,
                    CouponCode = "COUPON_2023_10",
                    DiscountAmount = 10
                },
                new Coupon
                {
                    Id = 2,
                    CouponCode = "COUPON_2023_15",
                    DiscountAmount = 15
                }
            };

            modelBuilder.Entity<Coupon>().HasData(coupons);
        }
    }
}
