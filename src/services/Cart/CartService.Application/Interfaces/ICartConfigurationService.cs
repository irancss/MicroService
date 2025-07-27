

namespace Cart.Application.Interfaces
{
    public interface ICartConfigurationService
    {
        Task<CartConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);
        Task UpdateConfigurationAsync(CartConfiguration configuration, CancellationToken cancellationToken = default);
    }
}
