using Microsoft.Extensions.Hosting; // اضافه کردن این using
using MassTransit;
using static MassTransit.MessageHeaders;

var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args) // حذف MessageHeaders.Host
    .ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("order-created-queue", e =>
                {
                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                });
            });
        });
    })
    .Build();

await host.RunAsync();