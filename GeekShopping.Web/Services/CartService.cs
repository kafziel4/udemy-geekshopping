using GeekShopping.Web.Exceptions;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class CartService : ICartService
    {
        public const string BaseUrl = "api/v1/carts";
        private readonly HttpClient _client;

        public CartService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<CartViewModel?> FindCartByUserId(string userId, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var query = new Dictionary<string, string?>()
            {
                ["userId"] = userId
            };
            var uri = QueryHelpers.AddQueryString(BaseUrl, query);
            var response = await _client.GetAsync(uri);
            if (response.StatusCode != HttpStatusCode.OK)
                return new CartViewModel();

            return await response.ReadContentAs<CartViewModel>();
        }

        public async Task<CartViewModel?> AddOrUpdateCart(CartViewModel model, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJsonAsync(BaseUrl, model);
            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<CartViewModel>();
        }

        public async Task<bool> RemoveFromCart(long cartDetailsId, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync($"{BaseUrl}/details/{cartDetailsId}");
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var query = new Dictionary<string, string?>()
            {
                ["userId"] = userId
            };
            var uri = QueryHelpers.AddQueryString($"{BaseUrl}/coupon", query);
            var response = await _client.PutAsJsonAsync(uri, couponCode);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> RemoveCoupon(string userId, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var query = new Dictionary<string, string?>()
            {
                ["userId"] = userId
            };
            var uri = QueryHelpers.AddQueryString($"{BaseUrl}/coupon", query);
            var response = await _client.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<CartHeaderViewModel?> Checkout(CartHeaderViewModel cartHeader, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJsonAsync($"{BaseUrl}/checkout", cartHeader);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartHeaderViewModel>();
            else if (response.StatusCode == HttpStatusCode.PreconditionFailed)
                throw new PreconditionException("Coupon Price has changed, please confirm!");
            else
                throw new HttpRequestException("Something went wrong when calling API;");
        }

        public Task<bool> ClearCart(string userId, string? token)
        {
            throw new NotImplementedException();
        }


    }
}
