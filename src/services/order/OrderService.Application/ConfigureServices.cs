using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using OrderService.Application.Mappings;
using OrderService.Application.Services;
using MediatR;
using OrderService.Application.Activities;
using OrderService.Application.Workflows;
using OrderService.Application.Sagas;
using MassTransit;
using Microsoft.Extensions.Configuration;
using OrderService.Application.Commands;
using FluentValidation;

namespace OrderService.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Ensure the correct implementation of OrderService is used
            services.AddScoped<IOrderService, OrderService.Application.Services.OrderService>();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly)
            );

            // TODO: Configure AutoMapper - version conflict to be resolved
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // MassTransit Configuration
            services.AddMassTransit(x =>
            {
                // Add the saga state machine
                x.AddSagaStateMachine<OrderSaga, OrderSagaState>()
                    .InMemoryRepository(); // For development - use real persistence in production

                // Add consumers
                x.AddConsumer<UpdateOrderStatusConsumer>();

                // Configure RabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfig = configuration.GetSection("RabbitMQ");
                    
                    cfg.Host(rabbitMqConfig["Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(rabbitMqConfig["Username"] ?? "guest");
                        h.Password(rabbitMqConfig["Password"] ?? "guest");
                    });

                    // Configure saga endpoint
                    cfg.ReceiveEndpoint("order-saga", e =>
                    {
                        e.ConfigureSaga<OrderSagaState>(context);
                    });

                    // Configure command handlers
                    cfg.ReceiveEndpoint("order-status-update", e =>
                    {
                        e.ConfigureConsumer<UpdateOrderStatusConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }

    // Consumer for handling UpdateOrderStatusCommand from saga
    public class UpdateOrderStatusConsumer : IConsumer<UpdateOrderStatusCommand>
    {
        private readonly IMediator _mediator;

        public UpdateOrderStatusConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UpdateOrderStatusCommand> context)
        {
            await _mediator.Send(context.Message);
        }
    }
}
