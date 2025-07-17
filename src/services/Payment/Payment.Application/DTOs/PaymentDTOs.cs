using Payment.Domain.ValueObjects;

namespace Payment.Application.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string GatewayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReferenceId { get; set; }
    public string? CardNumber { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class CreateTransactionDto
{
    public string OrderId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IRR";
    public string GatewayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string? Email { get; set; }
}

public class PaymentInitiationDto
{
    public string GatewayName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string? Email { get; set; }
}

public class PaymentVerificationDto
{
    public string GatewayName { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public Dictionary<string, string> GatewayData { get; set; } = new();
}

public class WalletDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WalletDepositDto
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IRR";
    public string Description { get; set; } = "Wallet deposit";
}

public class WalletWithdrawalDto
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IRR";
    public string Description { get; set; } = "Wallet withdrawal";
}

public class WalletPurchaseDto
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IRR";
    public string Description { get; set; } = "Purchase";
}

public class RefundRequestDto
{
    public Guid TransactionId { get; set; }
    public decimal? Amount { get; set; } // If null, full refund
    public string Reason { get; set; } = string.Empty;
}

public class GatewayInfoDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsTestMode { get; set; }
    public bool IsAvailable { get; set; }
}
