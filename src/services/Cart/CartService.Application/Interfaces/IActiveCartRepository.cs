using Cart.Domain.Entities;

namespace Cart.Application.Interfaces
{
    /// <summary>
    /// Repository for managing the active shopping cart (stored in Redis).
    /// </summary>
    public interface IActiveCartRepository
    {
        Task<ActiveCart?> GetByIdAsync(string cartId, CancellationToken cancellationToken = default);
        Task<ActiveCart> SaveAsync(ActiveCart cart, CancellationToken cancellationToken = default);
        Task DeleteAsync(string cartId, CancellationToken cancellationToken = default);
    }
}
