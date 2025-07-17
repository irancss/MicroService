using AuthService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add IdentityServer configuration
builder.Services.AddIdentityServer()
    .AddInMemoryClients(IdentityServerConfig.Clients)
    .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
    .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
    .AddDeveloperSigningCredential();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseIdentityServer();

app.UseAuthorization();

app.MapControllers();

app.Run();


