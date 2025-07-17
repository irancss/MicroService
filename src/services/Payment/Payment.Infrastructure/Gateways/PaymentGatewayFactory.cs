using Payment.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Payment.Infrastructure.Gateways;

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly IEnumerable<IPaymentGateway> _gateways;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentGatewayFactory> _logger;

    public PaymentGatewayFactory(
        IEnumerable<IPaymentGateway> gateways,
        IConfiguration configuration,
        ILogger<PaymentGatewayFactory> logger)
    {
        _gateways = gateways;
        _configuration = configuration;
        _logger = logger;
    }

    public IPaymentGateway GetGateway(string gatewayName)
    {
        var gateway = _gateways.FirstOrDefault(g => 
            g.GatewayName.Equals(gatewayName, StringComparison.OrdinalIgnoreCase));
        
        if (gateway == null)
        {
            _logger.LogError("Payment gateway '{GatewayName}' not found", gatewayName);
            throw new NotSupportedException($"درگاه پرداخت '{gatewayName}' یافت نشد");
        }
        
        return gateway;
    }

    public IEnumerable<IPaymentGateway> GetAvailableGateways(bool includeTestGateways = false)
    {
        var availableGateways = new List<IPaymentGateway>();
        
        foreach (var gateway in _gateways)
        {
            try
            {
                // Check if gateway is enabled in configuration
                var isEnabled = _configuration.GetValue<bool>($"PaymentGateways:{gateway.GatewayName}:IsEnabled", true);
                if (!isEnabled)
                {
                    _logger.LogDebug("Gateway '{GatewayName}' is disabled", gateway.GatewayName);
                    continue;
                }

                // Check test mode
                if (gateway.IsTestMode && !includeTestGateways)
                {
                    _logger.LogDebug("Gateway '{GatewayName}' is in test mode and test gateways are not included", gateway.GatewayName);
                    continue;
                }

                availableGateways.Add(gateway);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability of gateway '{GatewayName}'", gateway.GatewayName);
            }
        }

        return availableGateways;
    }
}

// Placeholder implementations for other Iranian gateways
public class SamanGateway : IPaymentGateway
{
    public string GatewayName => "Saman";
    public bool IsTestMode { get; private set; }

    public SamanGateway(IConfiguration configuration)
    {
        IsTestMode = configuration.GetValue<bool>("PaymentGateways:Saman:IsTest");
    }

    public Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request)
    {
        // TODO: Implement Saman gateway integration
        return Task.FromResult(new PaymentRequestResult
        {
            IsSuccess = false,
            ErrorMessage = "درگاه سامان هنوز پیاده‌سازی نشده است"
        });
    }

    public Task<PaymentVerificationResult> VerifyPaymentAsync(PaymentVerificationRequest request)
    {
        // TODO: Implement Saman verification
        return Task.FromResult(new PaymentVerificationResult
        {
            IsSuccess = false,
            ErrorMessage = "تایید پرداخت سامان هنوز پیاده‌سازی نشده است"
        });
    }

    public Task<RefundResult> RefundAsync(RefundRequest request)
    {
        // TODO: Implement Saman refund
        return Task.FromResult(new RefundResult
        {
            IsSuccess = false,
            ErrorMessage = "بازگشت وجه سامان هنوز پیاده‌سازی نشده است"
        });
    }
}

public class MellatGateway : IPaymentGateway
{
    public string GatewayName => "Mellat";
    public bool IsTestMode { get; private set; }

    public MellatGateway(IConfiguration configuration)
    {
        IsTestMode = configuration.GetValue<bool>("PaymentGateways:Mellat:IsTest");
    }

    public Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request)
    {
        // TODO: Implement Mellat gateway integration
        return Task.FromResult(new PaymentRequestResult
        {
            IsSuccess = false,
            ErrorMessage = "درگاه ملت هنوز پیاده‌سازی نشده است"
        });
    }

    public Task<PaymentVerificationResult> VerifyPaymentAsync(PaymentVerificationRequest request)
    {
        // TODO: Implement Mellat verification
        return Task.FromResult(new PaymentVerificationResult
        {
            IsSuccess = false,
            ErrorMessage = "تایید پرداخت ملت هنوز پیاده‌سازی نشده است"
        });
    }

    public Task<RefundResult> RefundAsync(RefundRequest request)
    {
        // TODO: Implement Mellat refund
        return Task.FromResult(new RefundResult
        {
            IsSuccess = false,
            ErrorMessage = "بازگشت وجه ملت هنوز پیاده‌سازی نشده است"
        });
    }
}

public class ParsianGateway : IPaymentGateway
{
    public string GatewayName => "Parsian";
    public bool IsTestMode { get; private set; }

    public ParsianGateway(IConfiguration configuration)
    {
        IsTestMode = configuration.GetValue<bool>("PaymentGateways:Parsian:IsTest");
    }

    public Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request)
    {
        return Task.FromResult(new PaymentRequestResult
        {
            IsSuccess = false,
            ErrorMessage = "درگاه پارسیان هنوز پیاده‌سازی نشده است"
        });
    }

    public Task<PaymentVerificationResult> VerifyPaymentAsync(PaymentVerificationRequest request)
    {
        return Task.FromResult(new PaymentVerificationResult
        {
            IsSuccess = false,
            ErrorMessage = "تایید پرداخت پارسیان هنوز پیاده‌سازی نشده است"
        });
    }

    public Task<RefundResult> RefundAsync(RefundRequest request)
    {
        return Task.FromResult(new RefundResult
        {
            IsSuccess = false,
            ErrorMessage = "بازگشت وجه پارسیان هنوز پیاده‌سازی نشده است"
        });
    }
}

public class IranKishGateway : IPaymentGateway
{
    public string GatewayName => "IranKish";
    public bool IsTestMode { get; private set; }

    public IranKishGateway(IConfiguration configuration)
    {
        IsTestMode = configuration.GetValue<bool>("PaymentGateways:IranKish:IsTest");
    }

    public Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request)
    {
        return Task.FromResult(new PaymentRequestResult
        {
            IsSuccess = false,
            ErrorMessage = "درگاه ایران کیش هنوز پیاده‌سازی نشده است"
        });
    }

    public Task<PaymentVerificationResult> VerifyPaymentAsync(PaymentVerificationRequest request)
    {
        return Task.FromResult(new PaymentVerificationResult
        {
            IsSuccess = false,
            ErrorMessage = "تایید پرداخت ایران کیش هنوز پیاده‌سازی نشده است"
        });
    }

    public Task<RefundResult> RefundAsync(RefundRequest request)
    {
        return Task.FromResult(new RefundResult
        {
            IsSuccess = false,
            ErrorMessage = "بازگشت وجه ایران کیش هنوز پیاده‌سازی نشده است"
        });
    }
}

// Add more gateway implementations as needed:
// - PasargadGateway
// - SepehrGateway  
// - DigipayGateway
// - SadadGateway
// - AsanPardakhtGateway
// - IndirectGateway
