using ShippingService.Domain.Enums;
using Shared.Kernel.Domain;

namespace ShippingService.Domain.Entities;

public class ShipmentReturn : AggregateRoot
{
    public Guid OriginalShipmentId { get; private set; }
    public Shipment OriginalShipment { get; private set; } = null!;
    public string CustomerId { get; private set; }
    public ReturnReason Reason { get; private set; }
    public string? ReasonDescription { get; private set; }
    public ReturnStatus Status { get; private set; }
    
    // Return Details
    public string? ReturnTrackingNumber { get; private set; }
    public DateTime RequestedDate { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public string? ApprovedByUserId { get; private set; }
    
    // Refund Information
    public decimal RefundAmount { get; private set; }
    public bool IsRefundProcessed { get; private set; }
    public DateTime? RefundProcessedDate { get; private set; }
    public string? RefundTransactionId { get; private set; }
    
    // Collection Information
    public string? CollectionAddress { get; private set; }
    public DateTime? CollectionDate { get; private set; }
    public string? CollectionNotes { get; private set; }

    private readonly List<ReturnTracking> _trackingHistory = new();
    public IReadOnlyCollection<ReturnTracking> TrackingHistory => _trackingHistory.AsReadOnly();

    protected ShipmentReturn() { }

    public ShipmentReturn(
        Guid originalShipmentId,
        string customerId,
        ReturnReason reason,
        string? reasonDescription = null,
        string? collectionAddress = null)
    {
        Id = Guid.NewGuid();
        OriginalShipmentId = originalShipmentId;
        CustomerId = customerId;
        Reason = reason;
        ReasonDescription = reasonDescription;
        Status = ReturnStatus.Requested;
        RequestedDate = DateTime.UtcNow;
        CollectionAddress = collectionAddress;
        CreatedAt = DateTime.UtcNow;
        
        GenerateReturnTrackingNumber();
        AddTrackingEvent(ReturnStatus.Requested, "Return request submitted");
    }

    public void Approve(string approvedByUserId, decimal refundAmount)
    {
        if (Status != ReturnStatus.Requested)
            throw new InvalidOperationException("Only requested returns can be approved");

        Status = ReturnStatus.Approved;
        ApprovedDate = DateTime.UtcNow;
        ApprovedByUserId = approvedByUserId;
        RefundAmount = refundAmount;
        UpdatedAt = DateTime.UtcNow;
        
        AddTrackingEvent(ReturnStatus.Approved, $"Return approved by {approvedByUserId}");
    }

    public void Reject(string rejectedByUserId, string reason)
    {
        if (Status != ReturnStatus.Requested)
            throw new InvalidOperationException("Only requested returns can be rejected");

        Status = ReturnStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
        
        AddTrackingEvent(ReturnStatus.Rejected, $"Return rejected by {rejectedByUserId}: {reason}");
    }

    public void StartCollection(DateTime collectionDate, string? notes = null)
    {
        if (Status != ReturnStatus.Approved)
            throw new InvalidOperationException("Only approved returns can start collection");

        Status = ReturnStatus.InTransit;
        CollectionDate = collectionDate;
        CollectionNotes = notes;
        UpdatedAt = DateTime.UtcNow;
        
        AddTrackingEvent(ReturnStatus.InTransit, "Item collection started");
    }

    public void CompleteReturn()
    {
        if (Status != ReturnStatus.InTransit)
            throw new InvalidOperationException("Only in-transit returns can be completed");

        Status = ReturnStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddTrackingEvent(ReturnStatus.Completed, "Return completed");
    }

    public void ProcessRefund(string transactionId)
    {
        if (Status != ReturnStatus.Completed)
            throw new InvalidOperationException("Refund can only be processed for completed returns");

        IsRefundProcessed = true;
        RefundProcessedDate = DateTime.UtcNow;
        RefundTransactionId = transactionId;
        UpdatedAt = DateTime.UtcNow;
        
        AddTrackingEvent(Status, $"Refund processed: {transactionId}");
    }

    public void UpdateStatus(ReturnStatus newStatus, string? notes = null)
    {
        if (Status == newStatus) return;
        
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        
        AddTrackingEvent(newStatus, notes);
    }

    private void GenerateReturnTrackingNumber()
    {
        ReturnTrackingNumber = $"RET{DateTime.Now:yyyyMMdd}{Id.ToString()[..8].ToUpper()}";
    }

    private void AddTrackingEvent(ReturnStatus status, string? notes)
    {
        var tracking = new ReturnTracking(Id, status, notes);
        _trackingHistory.Add(tracking);
    }
}
