using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class CouponService : ICouponService
    {
        public const string BaseUrl = "api/v1/coupons";
        private readonly HttpClient _client;

        public CouponService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<CouponViewModel> GetCoupon(string code, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"{BaseUrl}/{code}");
            return await response.ReadContentAs<CouponViewModel>() ?? new CouponViewModel();

        }
    }
}
