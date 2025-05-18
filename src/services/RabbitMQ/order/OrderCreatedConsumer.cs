using MassTransit;
using Contracts;
class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public Task Consume(ConsumeContext<IOrderCreated> context)
    {
        Console.WriteLine($"✅ Order Received: ID={context.Message.OrderId} at {context.Message.CreatedAt}");
        // مثلاً برو موجودی کم کن، یا لاگ بزن، یا ایمیل بفرست و ...
        return Task.CompletedTask;
    }
}