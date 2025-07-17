using FluentValidation;
using MediatR;
using NotificationService.Application.Behaviors;
using NotificationService.Infrastructure.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(NotificationService.Application.Commands.SendEmailCommand).Assembly);
});

// Add MediatR Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResilienceBehavior<,>));

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(NotificationService.Application.Validators.SendEmailCommandValidator).Assembly);

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddMongoDb(builder.Configuration.GetConnectionString("MongoDB")!, name: "mongodb")
    .AddRabbitMQ(builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672", name: "rabbitmq");

// Add Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Add Health Check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
