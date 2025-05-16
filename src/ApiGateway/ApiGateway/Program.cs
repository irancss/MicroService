using System.Threading.RateLimiting;
using ApiGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddMemoryCache();
builder.Services.AddLogging();


#region Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Audience = builder.Configuration["Identity:Authority"];
        options.Authority = "api-gateway";
    });

#endregion


#region Rate Limiting

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
         partitionKey: context.Request.Path.Value,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 100
            }));
    options.AddFixedWindowLimiter("api", options =>
    {
        options.Window = TimeSpan.FromMinutes(1);
        options.PermitLimit = 100;
    });
});
#endregion

builder.Services.AddTransient<LoggingHandler>();

builder.Services.AddHttpClient("Yarp.Cluster.productsCluster")
    .AddHttpMessageHandler<LoggingHandler>()
    .AddHttpMessageHandler(() => new CircuitBreakerHandler(3, TimeSpan.FromSeconds(30)));

builder.Services.AddHttpClient("Yarp.Cluster.usersCluster")
    .AddHttpMessageHandler<LoggingHandler>()
    .AddHttpMessageHandler(() => new CircuitBreakerHandler(3, TimeSpan.FromSeconds(30)));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builder =>
    {
        builder.AddRequestTransform(async transformContext =>
        {
            transformContext.ProxyRequest.Headers.Add("X-Custom-Header", "Value");

            if (transformContext.Path.StartsWithSegments("/old"))
            {
                transformContext.Path = transformContext.Path.Value?.Replace("/old", "/new");
            }
        });

        builder.AddResponseTransform(async transformContext =>
        {
            if (transformContext.ProxyResponse?.IsSuccessStatusCode == true)
            {
                transformContext.SuppressResponseBody = true;
                await transformContext.HttpContext.Response.WriteAsync("Custom Response");
            }
        });
    });



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseCacheMiddleware();
app.MapReverseProxy();

app.Run();
