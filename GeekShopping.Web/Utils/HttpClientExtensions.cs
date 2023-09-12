using System.Text.Json;

namespace GeekShopping.Web.Utils
{
    public static class HttpClientExtensions
    {
        public static async Task<T?> ReadContentAs<T>(this HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync<T>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
