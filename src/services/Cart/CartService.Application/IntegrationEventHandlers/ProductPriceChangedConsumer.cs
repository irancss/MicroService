using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Messaging.Events.Contracts;
using MassTransit;

namespace Cart.Application.IntegrationEventHandlers
{
    // این کلاس از IConsumer<T> ارث‌بری می‌کند که در BuildingBlocks با IEventHandler<T> معادل است
    public class ProductPriceChangedConsumer : IConsumer<ProductPriceChangedIntegrationEvent>
    {
        private readonly ILogger<ProductPriceChangedConsumer> _logger;
        // اینجا ریپازیتوری سبد خرید یا سرویس مربوطه را تزریق می‌کنید
        // private readonly ICartRepository _cartRepository; 

        public ProductPriceChangedConsumer(ILogger<ProductPriceChangedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received ProductPriceChanged event for ProductId: {ProductId}. New Price: {NewPrice}",
                message.ProductId,
                message.NewPrice);

            // منطق اصلی اینجا پیاده‌سازی می‌شود
            // 1. پیدا کردن تمام سبدهای خریدی که محصول با شناسه message.ProductId را دارند.
            // var cartsToUpdate = await _cartRepository.FindCartsContainingProductAsync(message.ProductId);

            // 2. به‌روزرسانی قیمت آیتم مربوطه در هر سبد خرید.
            // foreach (var cart in cartsToUpdate)
            // {
            //     cart.UpdateProductPrice(message.ProductId, message.NewPrice);
            //     await _cartRepository.UpdateAsync(cart);
            // }

            _logger.LogInformation("Successfully processed ProductPriceChanged event for ProductId: {ProductId}", message.ProductId);

            // await برای شبیه‌سازی کار
            await Task.CompletedTask;
        }
    }
}
