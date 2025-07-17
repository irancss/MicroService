using Payment.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Payment.Infrastructure.Gateways;

public class ZarinpalGateway : IPaymentGateway
{
    public string GatewayName => "Zarinpal";
    public bool IsTestMode { get; private set; }

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ZarinpalGateway> _logger;
    private readonly ZarinpalSettings _settings;

    public ZarinpalGateway(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ZarinpalGateway> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;

        var section = _configuration.GetSection("PaymentGateways:Zarinpal");
        IsTestMode = section.GetValue<bool>("IsTest");
        
        _settings = new ZarinpalSettings
        {
            IsTest = IsTestMode,
            MerchantId = section.GetValue<string>("MerchantId") ?? "",
            RequestUrl = section.GetValue<string>(IsTestMode ? "Test_RequestUrl" : "RequestUrl") ?? "",
            VerifyUrl = section.GetValue<string>(IsTestMode ? "Test_VerifyUrl" : "VerifyUrl") ?? "",
            PaymentRedirectUrl = section.GetValue<string>(IsTestMode ? "Test_PaymentRedirectUrl" : "PaymentRedirectUrl") ?? ""
        };
    }

    public async Task<PaymentRequestResult> RequestPaymentAsync(PaymentRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ZarinpalClient");
            
            var requestBody = new
            {
                merchant_id = _settings.MerchantId,
                amount = (int)request.Amount, // Zarinpal expects amount in Toman
                description = request.Description,
                callback_url = request.CallbackUrl,
                metadata = new { mobile = request.Mobile, email = request.Email }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending payment request to Zarinpal for Order {OrderId}", request.OrderId);

            var response = await client.PostAsync(_settings.RequestUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Zarinpal request failed with status {StatusCode}: {Response}", 
                    response.StatusCode, responseContent);
                return new PaymentRequestResult 
                { 
                    IsSuccess = false, 
                    ErrorMessage = "خطا در ارتباط با درگاه پرداخت زرین‌پال" 
                };
            }

            var result = JsonSerializer.Deserialize<ZarinpalRequestResponse>(responseContent);

            if (result?.data?.code == 100)
            {
                var paymentUrl = string.Format(_settings.PaymentRedirectUrl, result.data.authority);
                
                _logger.LogInformation("Zarinpal payment request successful for Order {OrderId}, Authority: {Authority}",
                    request.OrderId, result.data.authority);

                return new PaymentRequestResult
                {
                    IsSuccess = true,
                    PaymentUrl = paymentUrl,
                    GatewayTransactionId = result.data.authority
                };
            }

            var errorMessage = $"خطای زرین‌پال: {result?.errors?.code} - {result?.errors?.message}";
            _logger.LogWarning("Zarinpal payment request failed for Order {OrderId}: {Error}",
                request.OrderId, errorMessage);

            return new PaymentRequestResult 
            { 
                IsSuccess = false, 
                ErrorMessage = errorMessage 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in Zarinpal payment request for Order {OrderId}", request.OrderId);
            return new PaymentRequestResult 
            { 
                IsSuccess = false, 
                ErrorMessage = "خطای سیستمی در درگاه زرین‌پال" 
            };
        }
    }

    public async Task<PaymentVerificationResult> VerifyPaymentAsync(PaymentVerificationRequest request)
    {
        try
        {
            if (!request.GatewaySpecificData.TryGetValue("Authority", out var authority) && 
                string.IsNullOrEmpty(request.GatewayTransactionId))
            {
                return new PaymentVerificationResult 
                { 
                    IsSuccess = false, 
                    ErrorMessage = "کد رهگیری (Authority) یافت نشد" 
                };
            }

            authority ??= request.GatewayTransactionId;

            var client = _httpClientFactory.CreateClient("ZarinpalClient");
            
            var requestBody = new
            {
                merchant_id = _settings.MerchantId,
                amount = (int)request.Amount,
                authority = authority
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            _logger.LogInformation("Verifying payment with Zarinpal for Order {OrderId}, Authority: {Authority}",
                request.OrderId, authority);

            var response = await client.PostAsync(_settings.VerifyUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Zarinpal verification failed with status {StatusCode}: {Response}", 
                    response.StatusCode, responseContent);
                return new PaymentVerificationResult 
                { 
                    IsSuccess = false, 
                    ErrorMessage = "خطا در تایید تراکنش زرین‌پال" 
                };
            }

            var result = JsonSerializer.Deserialize<ZarinpalVerifyResponse>(responseContent);

            if (result?.data?.code == 100 || result?.data?.code == 101) // 101 means already verified
            {
                _logger.LogInformation("Zarinpal payment verified successfully for Order {OrderId}, RefId: {RefId}",
                    request.OrderId, result.data.ref_id);

                return new PaymentVerificationResult
                {
                    IsSuccess = true,
                    GatewayReferenceId = result.data.ref_id.ToString(),
                    CardNumber = result.data.card_pan
                };
            }

            var errorMessage = $"خطا در تایید: {result?.errors?.code} - {result?.errors?.message}";
            _logger.LogWarning("Zarinpal payment verification failed for Order {OrderId}: {Error}",
                request.OrderId, errorMessage);

            return new PaymentVerificationResult 
            { 
                IsSuccess = false, 
                ErrorMessage = errorMessage 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in Zarinpal payment verification for Order {OrderId}", request.OrderId);
            return new PaymentVerificationResult 
            { 
                IsSuccess = false, 
                ErrorMessage = "خطای سیستمی در تایید زرین‌پال" 
            };
        }
    }

    public async Task<RefundResult> RefundAsync(RefundRequest request)
    {
        // Zarinpal doesn't support automatic refunds via API
        // This would typically require manual processing
        
        _logger.LogInformation("Refund requested for Zarinpal transaction {TransactionId}, Amount: {Amount}",
            request.GatewayTransactionId, request.Amount);

        await Task.CompletedTask;

        return new RefundResult
        {
            IsSuccess = false,
            ErrorMessage = "بازگشت وجه در زرین‌پال نیازمند پردازش دستی است"
        };
    }
}

// Helper classes for Zarinpal API responses
public class ZarinpalRequestResponse
{
    public ZarinpalData? data { get; set; }
    public ZarinpalError? errors { get; set; }
}

public class ZarinpalVerifyResponse
{
    public ZarinpalData? data { get; set; }
    public ZarinpalError? errors { get; set; }
}

public class ZarinpalData
{
    public int code { get; set; }
    public string? authority { get; set; }
    public long ref_id { get; set; }
    public string? card_pan { get; set; }
}

public class ZarinpalError
{
    public int code { get; set; }
    public string? message { get; set; }
}

public class ZarinpalSettings
{
    public bool IsTest { get; set; }
    public string MerchantId { get; set; } = string.Empty;
    public string RequestUrl { get; set; } = string.Empty;
    public string VerifyUrl { get; set; } = string.Empty;
    public string PaymentRedirectUrl { get; set; } = string.Empty;
}
