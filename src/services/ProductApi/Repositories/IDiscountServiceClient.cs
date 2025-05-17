namespace ProductApi.Services
{
    /// <summary>
    /// Interface for communicating with the Discount Service.
    /// </summary>
    public interface IDiscountServiceClient
    {
        /// <summary>
        /// Gets the current discount percentage or fixed amount for a product by SKU or Id.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <returns>Discount value or null if no discount is available.</returns>
        Task<decimal?> GetCurrentDiscountAsync(string productId);
    }

    public class DiscountServiceClient : IDiscountServiceClient
    {
        private readonly HttpClient _httpClient;
        public DiscountServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<decimal?> GetCurrentDiscountAsync(string productId)
        {
            var response = await _httpClient.GetAsync($"discounts/{productId}");
            if (response.IsSuccessStatusCode)
            {
                var discount = await response.Content.ReadFromJsonAsync<decimal?>();
                return discount;
            }
            return null;
        }
    }
}
