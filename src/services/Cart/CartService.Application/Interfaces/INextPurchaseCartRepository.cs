using Cart.Domain.Entities;

namespace Cart.Application.Interfaces
{
    /// <summary>
    /// Repository for managing the next-purchase cart (stored in PostgreSQL).
    /// </summary>
    public interface INextPurchaseCartRepository
    {
        Task<NextPurchaseCart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<NextPurchaseCart> SaveAsync(NextPurchaseCart cart, CancellationToken cancellationToken = default);
        Task DeleteAsync(string userId, CancellationToken cancellationToken = default);
    }
}
