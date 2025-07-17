using Payment.Domain.Common;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Entities;

public class ReconciliationReport : BaseEntity
{
    public DateTime ReportDate { get; private set; }
    public GatewayName GatewayName { get; private set; }
    public int TotalTransactionsCount { get; private set; }
    public Money TotalAmount { get; private set; }
    public int SuccessfulTransactionsCount { get; private set; }
    public Money SuccessfulAmount { get; private set; }
    public int FailedTransactionsCount { get; private set; }
    public int MismatchedTransactionsCount { get; private set; }
    public ReconciliationStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public ICollection<ReconciliationMismatch> Mismatches { get; private set; } = new List<ReconciliationMismatch>();

    public ReconciliationReport(
        DateTime reportDate,
        GatewayName gatewayName,
        int totalTransactionsCount,
        Money totalAmount,
        int successfulTransactionsCount,
        Money successfulAmount,
        int failedTransactionsCount)
    {
        ReportDate = reportDate;
        GatewayName = gatewayName;
        TotalTransactionsCount = totalTransactionsCount;
        TotalAmount = totalAmount;
        SuccessfulTransactionsCount = successfulTransactionsCount;
        SuccessfulAmount = successfulAmount;
        FailedTransactionsCount = failedTransactionsCount;
        Status = ReconciliationStatus.InProgress;
    }

    // Private constructor for EF Core
    private ReconciliationReport() 
    { 
        GatewayName = default!;
        TotalAmount = default!;
        SuccessfulAmount = default!;
    }

    public void AddMismatch(ReconciliationMismatch mismatch)
    {
        Mismatches.Add(mismatch);
        MismatchedTransactionsCount = Mismatches.Count;
        MarkAsUpdated();
    }

    public void MarkAsCompleted(string? notes = null)
    {
        Status = ReconciliationStatus.Completed;
        Notes = notes;
        MarkAsUpdated();
    }

    public void MarkAsFailed(string errorReason)
    {
        Status = ReconciliationStatus.Failed;
        Notes = errorReason;
        MarkAsUpdated();
    }
}

public class ReconciliationMismatch : BaseEntity
{
    public Guid ReconciliationReportId { get; private set; }
    public TransactionId TransactionId { get; private set; }
    public MismatchType Type { get; private set; }
    public string Description { get; private set; }
    public Money? ExpectedAmount { get; private set; }
    public Money? ActualAmount { get; private set; }
    public TransactionStatus? ExpectedStatus { get; private set; }
    public TransactionStatus? ActualStatus { get; private set; }
    public bool IsResolved { get; private set; }
    public string? Resolution { get; private set; }

    public ReconciliationMismatch(
        Guid reconciliationReportId,
        TransactionId transactionId,
        MismatchType type,
        string description)
    {
        ReconciliationReportId = reconciliationReportId;
        TransactionId = transactionId;
        Type = type;
        Description = description;
        IsResolved = false;
    }

    // Private constructor for EF Core
    private ReconciliationMismatch() 
    { 
        TransactionId = default!;
        Description = string.Empty;
    }

    public void SetAmountMismatch(Money expected, Money actual)
    {
        ExpectedAmount = expected;
        ActualAmount = actual;
    }

    public void SetStatusMismatch(TransactionStatus expected, TransactionStatus actual)
    {
        ExpectedStatus = expected;
        ActualStatus = actual;
    }

    public void Resolve(string resolution)
    {
        IsResolved = true;
        Resolution = resolution;
        MarkAsUpdated();
    }
}

public enum ReconciliationStatus
{
    InProgress = 1,
    Completed = 2,
    Failed = 3
}

public enum MismatchType
{
    AmountMismatch = 1,
    StatusMismatch = 2,
    MissingTransaction = 3,
    ExtraTransaction = 4
}
