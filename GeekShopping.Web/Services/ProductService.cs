using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class ProductService : IProductService
    {
        public const string BaseUrl = "api/v1/products";
        private readonly HttpClient _client;

        public ProductService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<ProductViewModel>> FindAllProducts(string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<List<ProductViewModel>>() ?? new List<ProductViewModel>();
        }

        public async Task<ProductViewModel?> FindProductById(long id, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<ProductViewModel>();
        }

        public async Task<ProductViewModel?> CreateProduct(ProductViewModel model, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJsonAsync(BaseUrl, model);
            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<ProductViewModel>();
        }

        public async Task<ProductViewModel?> UpdateProduct(ProductViewModel model, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{model.Id}", model);
            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<ProductViewModel>();
        }

        public async Task<bool> DeleteProductById(long id, string? token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
