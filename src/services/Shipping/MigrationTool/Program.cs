using Microsoft.EntityFrameworkCore;
using ShippingService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<ShippingDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=ShippingServiceDb;Username=postgres;Password=123"));

var app = builder.Build();

app.MapGet("/", () => "Shipping Service Migration Tool");

app.Run();
