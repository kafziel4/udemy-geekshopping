using GeekShopping.CartAPI.Data.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GeekShopping.CartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        public const string BaseUrl = "api/v1/coupons";
        private readonly HttpClient _client;

        public CouponRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<CouponDto> GetCoupon(string couponCode, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"{BaseUrl}/{couponCode}");

            return await response.Content.ReadFromJsonAsync<CouponDto>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
                new CouponDto();
        }
    }
}
