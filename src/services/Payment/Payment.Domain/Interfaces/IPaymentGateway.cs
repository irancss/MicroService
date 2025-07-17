using Payment.Domain.ValueObjects;

namespace Payment.Domain.Interfaces;

public interface IPaymentGateway
{
    string GatewayName { get; }
    bool IsTestMode { get; }
    Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request);
    Task<PaymentVerificationResult> VerifyPaymentAsync(PaymentVerificationRequest request);
    Task<RefundResult> RefundAsync(RefundRequest request);
}

public interface IPaymentGatewayFactory
{
    IPaymentGateway GetGateway(string gatewayName);
    IEnumerable<IPaymentGateway> GetAvailableGateways(bool includeTestGateways = false);
}

// Gateway Request/Response Models
public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string? Email { get; set; }
}

public class PaymentRequestResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? PaymentUrl { get; set; }
    public string? GatewayTransactionId { get; set; }
}

public class PaymentVerificationRequest
{
    public decimal Amount { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string GatewayTransactionId { get; set; } = string.Empty;
    public Dictionary<string, string> GatewaySpecificData { get; set; } = new();
}

public class PaymentVerificationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? GatewayReferenceId { get; set; }
    public string? CardNumber { get; set; }
}

public class RefundRequest
{
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string GatewayReferenceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class RefundResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RefundId { get; set; }
}
