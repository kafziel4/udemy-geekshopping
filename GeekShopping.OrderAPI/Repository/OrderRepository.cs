using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<MySqlContext> _context;

        public OrderRepository(DbContextOptions<MySqlContext> context)
        {
            _context = context;
        }

        public async Task<bool> AddOrder(OrderHeader header)
        {
            await using var db = new MySqlContext(_context);
            db.Headers.Add(header);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(long orderHeaderId, bool paymentStatus)
        {
            await using var db = new MySqlContext(_context);
            var header = await db.Headers.FindAsync(orderHeaderId);
            if (header is not null)
            {
                header.PaymentStatus = paymentStatus;
                await db.SaveChangesAsync();
            }
        }
    }
}
