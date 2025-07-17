using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using System.Text.Json;

namespace Payment.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<ReconciliationReport> ReconciliationReports { get; set; }
    public DbSet<ReconciliationMismatch> ReconciliationMismatches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Value Converters
        var transactionIdConverter = new ValueConverter<TransactionId, Guid>(
            v => v.Value,
            v => new TransactionId(v));

        var orderIdConverter = new ValueConverter<OrderId, string>(
            v => v.Value,
            v => new OrderId(v));

        var userIdConverter = new ValueConverter<UserId, Guid>(
            v => v.Value,
            v => new UserId(v));

        var gatewayNameConverter = new ValueConverter<GatewayName, string>(
            v => v.Value,
            v => new GatewayName(v));

        var moneyConverter = new ValueConverter<Money, string>(
            v => JsonSerializer.Serialize(new { Amount = v.Amount, Currency = v.Currency.ToString() }, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<MoneyData>(v, (JsonSerializerOptions?)null) != null ? 
                new Money(JsonSerializer.Deserialize<MoneyData>(v, (JsonSerializerOptions?)null)!.Amount, 
                         Enum.Parse<Currency>(JsonSerializer.Deserialize<MoneyData>(v, (JsonSerializerOptions?)null)!.Currency)) :
                new Money(0, Currency.IRR));

        // Transaction Configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TransactionId)
                .HasConversion(transactionIdConverter)
                .IsRequired();

            entity.Property(e => e.OrderId)
                .HasConversion(orderIdConverter)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasConversion(userIdConverter)
                .IsRequired();

            entity.Property(e => e.Amount)
                .HasConversion(moneyConverter)
                .IsRequired();

            entity.Property(e => e.GatewayName)
                .HasConversion(gatewayNameConverter)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.CallbackUrl)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.GatewayTransactionId)
                .HasMaxLength(100);

            entity.Property(e => e.GatewayReferenceId)
                .HasMaxLength(100);

            entity.Property(e => e.CardNumber)
                .HasMaxLength(20);

            // Relationships
            entity.HasOne(e => e.ParentTransaction)
                .WithMany(e => e.RefundTransactions)
                .HasForeignKey(e => e.ParentTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.TransactionId).IsUnique();
            entity.HasIndex(e => e.OrderId).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.GatewayName);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Wallet Configuration
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId)
                .HasConversion(userIdConverter)
                .IsRequired();

            entity.Property(e => e.Balance)
                .HasConversion(moneyConverter)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Relationships
            entity.HasMany(e => e.Transactions)
                .WithOne()
                .HasForeignKey("WalletId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        // Wallet Transaction Configuration
        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserId)
                .HasConversion(userIdConverter)
                .IsRequired();

            entity.Property(e => e.Amount)
                .HasConversion(moneyConverter)
                .IsRequired();

            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsRequired();

            // Indexes
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Reconciliation Report Configuration
        modelBuilder.Entity<ReconciliationReport>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.GatewayName)
                .HasConversion(gatewayNameConverter)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.TotalAmount)
                .HasConversion(moneyConverter)
                .IsRequired();

            entity.Property(e => e.SuccessfulAmount)
                .HasConversion(moneyConverter)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasMaxLength(1000);

            // Relationships
            entity.HasMany(e => e.Mismatches)
                .WithOne()
                .HasForeignKey(e => e.ReconciliationReportId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.ReportDate, e.GatewayName }).IsUnique();
        });

        // Reconciliation Mismatch Configuration
        modelBuilder.Entity<ReconciliationMismatch>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TransactionId)
                .HasConversion(transactionIdConverter)
                .IsRequired();

            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.ExpectedAmount)
                .HasConversion(moneyConverter);

            entity.Property(e => e.ActualAmount)
                .HasConversion(moneyConverter);

            entity.Property(e => e.ExpectedStatus)
                .HasConversion<string?>()
                .HasMaxLength(20);

            entity.Property(e => e.ActualStatus)
                .HasConversion<string?>()
                .HasMaxLength(20);

            entity.Property(e => e.Resolution)
                .HasMaxLength(500);
        });
    }

    private class MoneyData
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
