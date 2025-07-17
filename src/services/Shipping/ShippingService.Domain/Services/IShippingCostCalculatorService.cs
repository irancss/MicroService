using ShippingService.Domain.Entities;

namespace ShippingService.Domain.Services
{
    /// <summary>
    /// Service interface for calculating shipping costs
    /// خدمات محاسبه هزینه حمل و نقل
    /// </summary>
    public interface IShippingCostCalculatorService
    {
        /// <summary>
        /// Calculate shipping cost for a shipment
        /// محاسبه هزینه ارسال برای یک مرسوله
        /// </summary>
        /// <param name="originAddress">آدرس مبدا</param>
        /// <param name="destinationAddress">آدرس مقصد</param>
        /// <param name="weight">وزن (کیلوگرم)</param>
        /// <param name="dimensions">ابعاد بسته</param>
        /// <param name="shippingMethodId">شناسه روش ارسال</param>
        /// <returns>هزینه محاسبه شده</returns>
        Task<decimal> CalculateShippingCostAsync(
            string originAddress,
            string destinationAddress,
            decimal weight,
            string dimensions,
            Guid shippingMethodId);

        /// <summary>
        /// Calculate estimated delivery time
        /// محاسبه زمان تحویل تخمینی
        /// </summary>
        /// <param name="originAddress">آدرس مبدا</param>
        /// <param name="destinationAddress">آدرس مقصد</param>
        /// <param name="shippingMethodId">شناسه روش ارسال</param>
        /// <returns>زمان تحویل تخمینی</returns>
        Task<TimeSpan> CalculateDeliveryTimeAsync(
            string originAddress,
            string destinationAddress,
            Guid shippingMethodId);

        /// <summary>
        /// Get available shipping methods for a route
        /// دریافت روش‌های ارسال موجود برای یک مسیر
        /// </summary>
        /// <param name="originAddress">آدرس مبدا</param>
        /// <param name="destinationAddress">آدرس مقصد</param>
        /// <param name="weight">وزن</param>
        /// <returns>لیست روش‌های ارسال موجود</returns>
        Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync(
            string originAddress,
            string destinationAddress,
            decimal weight);
    }
}
