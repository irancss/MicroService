using Contracts;
using MassTransit;

public class OrderCreatedConsumer : IConsumer<IOrderCreated>
{
    public Task Consume(ConsumeContext<IOrderCreated> context)
    {
        Console.WriteLine($"Order received: {context.Message.OrderId}");
        // انجام عملیات مورد نیاز مانند ارسال ایمیل یا به‌روزرسانی موجودی
        return Task.CompletedTask;
    }
}