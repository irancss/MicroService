using System.Threading.RateLimiting;
using ApiGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// <summary>
// Adds memory caching services to the dependency injection container.
// </summary>
builder.Services.AddMemoryCache();

// <summary>
// Adds logging services to the dependency injection container.
// </summary>
builder.Services.AddLogging();


#region Authentication

// <summary>
// Configures JWT Bearer authentication for the application.
// It sets the default authentication scheme to JwtBearerDefaults.AuthenticationScheme.
// The JWT token's audience and authority are configured based on the application's configuration settings.
// </summary>
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Specifies the audience for the JWT token, typically the API Gateway itself or a specific resource.
        options.Audience = builder.Configuration["Identity:Authority"];
        // Specifies the authority that issues the JWT tokens.
        options.Authority = "api-gateway"; // This seems like it should also come from configuration or be a more specific issuer URI.
    });

#endregion


#region Rate Limiting

// <summary>
// Configures rate limiting services for the application.
// This setup includes a global rate limiter and a named fixed window rate limiter.
// </summary>
builder.Services.AddRateLimiter(options =>
{
    // <summary>
    // Configures a global rate limiter that applies to all requests.
    // It uses a PartitionedRateLimiter based on the request path.
    // Each unique request path gets its own fixed window limiter.
    // </summary>
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
         partitionKey: context.Request.Path.Value ?? string.Empty, // Ensure partitionKey is not null
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1), // The time window for the rate limit.
                PermitLimit = 100 // The maximum number of requests allowed within the time window.
            }));

    // <summary>
    // Adds a specific named fixed window rate limiter policy called "api".
    // This can be applied selectively to specific endpoints or groups of endpoints.
    // </summary>
    options.AddFixedWindowLimiter("api", options =>
    {
        options.Window = TimeSpan.FromMinutes(1); // The time window for this specific policy.
        options.PermitLimit = 100; // The maximum number of requests for this policy.
    });
});
#endregion

// <summary>
// Registers the LoggingHandler as a transient service.
// This handler can be used to log HTTP request and response details.
// </summary>
builder.Services.AddTransient<LoggingHandler>();

// <summary>
// Configures an HttpClient for the "productsCluster" used by YARP.
// It adds a LoggingHandler for request/response logging.
// It also adds a CircuitBreakerHandler to implement the circuit breaker pattern,
// configured to break after 3 consecutive failures and stay open for 30 seconds.
// </summary>
builder.Services.AddHttpClient("Yarp.Cluster.productsCluster")
    .AddHttpMessageHandler<LoggingHandler>()
    .AddHttpMessageHandler(() => new CircuitBreakerHandler(3, TimeSpan.FromSeconds(30)));

// <summary>
// Configures an HttpClient for the "usersCluster" used by YARP.
// It adds a LoggingHandler for request/response logging.
// It also adds a CircuitBreakerHandler to implement the circuit breaker pattern,
// configured to break after 3 consecutive failures and stay open for 30 seconds.
// </summary>
builder.Services.AddHttpClient("Yarp.Cluster.usersCluster")
    .AddHttpMessageHandler<LoggingHandler>()
    .AddHttpMessageHandler(() => new CircuitBreakerHandler(3, TimeSpan.FromSeconds(30)));

// <summary>
// Adds and configures the YARP reverse proxy.
// The proxy configuration is loaded from the "ReverseProxy" section of the application's configuration.
// Custom request and response transformations are also added.
// </summary>
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext => // Renamed 'builder' to 'builderContext' to avoid conflict with WebApplicationBuilder
    {
        // <summary>
        // Adds a request transformation.
        // This transform adds a custom header "X-Custom-Header" to the outgoing proxy request.
        // It also rewrites the request path if it starts with "/old", changing it to "/new".
        // </summary>
        builderContext.AddRequestTransform(async transformContext =>
        {
            transformContext.ProxyRequest.Headers.Add("X-Custom-Header", "Value");

            if (transformContext.Path.StartsWithSegments("/old"))
            {
                transformContext.Path = transformContext.Path.Value?.Replace("/old", "/new");
            }
        });

        // <summary>
        // Adds a response transformation.
        // If the proxy response was successful (IsSuccessStatusCode is true),
        // this transform suppresses the original response body and writes a custom response.
        // </summary>
        builderContext.AddResponseTransform(async transformContext =>
        {
            if (transformContext.ProxyResponse?.IsSuccessStatusCode == true)
            {
                transformContext.SuppressResponseBody = true;
                await transformContext.HttpContext.Response.WriteAsync("Custom Response");
            }
        });
    });


// <summary>
// Adds services for controllers to the dependency injection container.
// </summary>
builder.Services.AddControllers();

// <summary>
// Adds API Explorer services, which are used by Swagger/OpenAPI for generating API documentation.
// </summary>
builder.Services.AddEndpointsApiExplorer();

// <summary>
// Adds Swagger generation services to the dependency injection container.
// </summary>
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

// <summary>
// Configures Swagger and Swagger UI middleware if the application is running in the Development environment.
// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// <summary>
// Adds middleware for redirecting HTTP requests to HTTPS.
// </summary>
app.UseHttpsRedirection();

// <summary>
// Adds authentication middleware to the pipeline.
// This enables authentication capabilities based on the configured schemes.
// </summary>
app.UseAuthentication();

// <summary>
// Adds authorization middleware to the pipeline.
// This enables authorization capabilities, ensuring users have the necessary permissions.
// </summary>
app.UseAuthorization();

// <summary>
// Adds the rate limiter middleware to the pipeline.
// This enforces the rate limiting policies configured earlier.
// </summary>
app.UseRateLimiter();

// <summary>
// Adds a custom cache middleware to the pipeline.
// (Assuming CacheMiddleware is a custom middleware for caching responses).
// </summary>
app.UseCacheMiddleware(); // Assuming CacheMiddleware is defined elsewhere

// <summary>
// Maps the YARP reverse proxy endpoints.
// This middleware handles incoming requests and forwards them to the appropriate backend services
// as configured in the YARP settings.
// </summary>
app.MapReverseProxy();

app.Run();