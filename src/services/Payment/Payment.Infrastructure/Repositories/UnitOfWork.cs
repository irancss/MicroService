using Microsoft.EntityFrameworkCore.Storage;
using Payment.Domain.Interfaces;
using Payment.Infrastructure.Data;

namespace Payment.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PaymentDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Lazy<ITransactionRepository> _transactions;
    private readonly Lazy<IWalletRepository> _wallets;
    private readonly Lazy<IReconciliationRepository> _reconciliations;

    public UnitOfWork(PaymentDbContext context)
    {
        _context = context;
        _transactions = new Lazy<ITransactionRepository>(() => new TransactionRepository(_context));
        _wallets = new Lazy<IWalletRepository>(() => new WalletRepository(_context));
        _reconciliations = new Lazy<IReconciliationRepository>(() => new ReconciliationRepository(_context));
    }

    public ITransactionRepository Transactions => _transactions.Value;
    public IWalletRepository Wallets => _wallets.Value;
    public IReconciliationRepository Reconciliations => _reconciliations.Value;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        await _context.DisposeAsync();
    }
}
