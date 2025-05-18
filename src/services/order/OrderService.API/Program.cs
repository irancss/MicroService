using Microsoft.OpenApi.Models;
using OrderService.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// SignalR
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = true;
});

// Dapr
builder.Services.AddDaprClient();
builder.Services.AddControllers().AddNewtonsoftJson();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Service API", Version = "v1" });
});

var app = builder.Build();

// Middlewares
app.UseCloudEvents();
app.MapSubscribeHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapHub<OrderHub>("/orderhub");

app.Run();