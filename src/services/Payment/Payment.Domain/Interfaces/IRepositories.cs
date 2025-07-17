using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<Transaction?> GetByTransactionIdAsync(TransactionId transactionId);
    Task<Transaction?> GetByOrderIdAsync(OrderId orderId);
    Task<IEnumerable<Transaction>> GetByUserIdAsync(UserId userId, int page = 1, int pageSize = 10);
    Task<IEnumerable<Transaction>> GetByGatewayAsync(GatewayName gatewayName, DateTime from, DateTime to);
    Task<IEnumerable<Transaction>> GetPendingTransactionsAsync();
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task<bool> ExistsAsync(TransactionId transactionId);
}

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(UserId userId);
    Task<Wallet?> GetByIdAsync(Guid id);
    Task AddAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
    Task<bool> ExistsForUserAsync(UserId userId);
}

public interface IReconciliationRepository
{
    Task<ReconciliationReport?> GetByIdAsync(Guid id);
    Task<ReconciliationReport?> GetByDateAndGatewayAsync(DateTime date, GatewayName gatewayName);
    Task<IEnumerable<ReconciliationReport>> GetReportsAsync(DateTime from, DateTime to);
    Task AddAsync(ReconciliationReport report);
    Task UpdateAsync(ReconciliationReport report);
}

public interface IUnitOfWork
{
    ITransactionRepository Transactions { get; }
    IWalletRepository Wallets { get; }
    IReconciliationRepository Reconciliations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
