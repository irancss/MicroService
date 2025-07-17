using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Domain.Interfaces;
using Payment.Domain.ValueObjects;
using Payment.Infrastructure.Data;

namespace Payment.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly PaymentDbContext _context;

    public TransactionRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions
            .Include(t => t.RefundTransactions)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction?> GetByTransactionIdAsync(TransactionId transactionId)
    {
        return await _context.Transactions
            .Include(t => t.RefundTransactions)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task<Transaction?> GetByOrderIdAsync(OrderId orderId)
    {
        return await _context.Transactions
            .Include(t => t.RefundTransactions)
            .FirstOrDefaultAsync(t => t.OrderId == orderId);
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(UserId userId, int page = 1, int pageSize = 10)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByGatewayAsync(GatewayName gatewayName, DateTime from, DateTime to)
    {
        return await _context.Transactions
            .Where(t => t.GatewayName == gatewayName && 
                       t.CreatedAt >= from && 
                       t.CreatedAt <= to)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetPendingTransactionsAsync()
    {
        return await _context.Transactions
            .Where(t => t.Status == TransactionStatus.Pending)
            .ToListAsync();
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(TransactionId transactionId)
    {
        return await _context.Transactions
            .AnyAsync(t => t.TransactionId == transactionId);
    }
}

public class WalletRepository : IWalletRepository
{
    private readonly PaymentDbContext _context;

    public WalletRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByUserIdAsync(UserId userId)
    {
        return await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task<Wallet?> GetByIdAsync(Guid id)
    {
        return await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task AddAsync(Wallet wallet)
    {
        await _context.Wallets.AddAsync(wallet);
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsForUserAsync(UserId userId)
    {
        return await _context.Wallets
            .AnyAsync(w => w.UserId == userId);
    }
}

public class ReconciliationRepository : IReconciliationRepository
{
    private readonly PaymentDbContext _context;

    public ReconciliationRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<ReconciliationReport?> GetByIdAsync(Guid id)
    {
        return await _context.ReconciliationReports
            .Include(r => r.Mismatches)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<ReconciliationReport?> GetByDateAndGatewayAsync(DateTime date, GatewayName gatewayName)
    {
        return await _context.ReconciliationReports
            .Include(r => r.Mismatches)
            .FirstOrDefaultAsync(r => r.ReportDate.Date == date.Date && r.GatewayName == gatewayName);
    }

    public async Task<IEnumerable<ReconciliationReport>> GetReportsAsync(DateTime from, DateTime to)
    {
        return await _context.ReconciliationReports
            .Include(r => r.Mismatches)
            .Where(r => r.ReportDate >= from && r.ReportDate <= to)
            .OrderByDescending(r => r.ReportDate)
            .ToListAsync();
    }

    public async Task AddAsync(ReconciliationReport report)
    {
        await _context.ReconciliationReports.AddAsync(report);
    }

    public async Task UpdateAsync(ReconciliationReport report)
    {
        _context.ReconciliationReports.Update(report);
        await Task.CompletedTask;
    }
}
