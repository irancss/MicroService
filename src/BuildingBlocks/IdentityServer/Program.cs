using IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityServer8.EntityFramework.DbContexts;
using IdentityServer8.EntityFramework.Mappers;
using System.Security.Cryptography.X509Certificates;
using BuildingBlocks.Observability;
using BuildingBlocks.Resiliency;

// Alias للجلوگیری از تداخل نام‌ها
using IdentityApplicationDbContext = IdentityServer.Data.ApplicationDbContext;
using IdentityApplicationUser = IdentityServer.Models.ApplicationUser;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddDbContext<IdentityApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<IdentityApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<IdentityApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    
    // Cookie settings for proper authentication
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
    ? CookieSecurePolicy.None
    : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax; // More permissive than None
    options.Cookie.Name = "IdentityServer.Auth";
    options.Cookie.IsEssential = true; // Essential for authentication
});

// Configure all cookie policies
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None;
    options.Secure = CookieSecurePolicy.None;
});

// Add IdentityServer8
var identityServerBuilder = builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = builder.Configuration["IdentityServer:IssuerUri"];
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    
    // Cookie settings for IdentityServer
    options.Authentication.CookieSameSiteMode = SameSiteMode.Lax;
    options.Authentication.RequireAuthenticatedUserForSignOutMessage = false;
})
.AddConfigurationStore(options =>
{
    options.ConfigureDbContext = b => b.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly(typeof(Program).Assembly.FullName));
})
.AddOperationalStore(options =>
{
    options.ConfigureDbContext = b => b.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly(typeof(Program).Assembly.FullName));
    options.EnableTokenCleanup = true;
})
.AddAspNetIdentity<IdentityApplicationUser>();

// Add signing credential
if (builder.Environment.IsDevelopment())
{
    identityServerBuilder.AddDeveloperSigningCredential();
}
else
{
     // Load from file
    var certPath = builder.Configuration["IdentityServer:SigningCertificate:Path"];
    var certPassword = builder.Configuration["IdentityServer:SigningCertificate:Password"];
    identityServerBuilder.AddSigningCredential(new X509Certificate2(certPath, certPassword));
}

// Add SMS Service
builder.Services.AddSingleton<ISmsService, SmsService>();

// Add BuildingBlocks Services
//builder.Services.AddServiceMesh(builder.Configuration);
builder.Services.AddObservability(builder.Configuration);
builder.Services.AddResiliency(builder.Configuration);

// Add MVC
builder.Services.AddControllersWithViews();;

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001", "http://localhost:7000", "https://localhost:7001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapControllerRoute("debug", "debug/{controller}/{action}");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use BuildingBlocks Middleware
app.UseObservability();

// Use cookie policy
app.UseCookiePolicy();

app.UseRouting();
app.UseCors();

// IdentityServer middleware - باید قبل از Authentication باشد
app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health");

// Initialize database
await InitializeDatabaseAsync(app.Services);

Log.Information("سرور IdentityServer8 شروع شد");

app.Run();

static async Task InitializeDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var scopedServices = scope.ServiceProvider;
    try
    {
        var context = scopedServices.GetRequiredService<IdentityApplicationDbContext>();
        var configContext = scopedServices.GetRequiredService<ConfigurationDbContext>();
        var persistedContext = scopedServices.GetRequiredService<PersistedGrantDbContext>();
         var configuration = scopedServices.GetRequiredService<IConfiguration>(); 
        
        var adminPassword = configuration["AdminUser:DefaultPassword"]; // <--- این را اضافه کنید
        if (string.IsNullOrEmpty(adminPassword))
        {
            Log.Error("رمز عبور پیش‌فرض ادمین در تنظیمات یافت نشد. (AdminUser:DefaultPassword)");
            throw new InvalidOperationException("Default admin password is not set in configuration.");
        }

        // Migrate databases
        await context.Database.MigrateAsync();
        await configContext.Database.MigrateAsync();
        await persistedContext.Database.MigrateAsync();
        
        // Seed configuration data
        if (!await configContext.Clients.AnyAsync())
        {
            foreach (var client in IdentityServer.Config.Clients)
            {
                configContext.Clients.Add(client.ToEntity());
            }
            await configContext.SaveChangesAsync();
        }

        if (!await configContext.IdentityResources.AnyAsync())
        {
            foreach (var resource in IdentityServer.Config.IdentityResources)
            {
                configContext.IdentityResources.Add(resource.ToEntity());
            }
            await configContext.SaveChangesAsync();
        }

        if (!await configContext.ApiScopes.AnyAsync())
        {
            foreach (var apiScope in IdentityServer.Config.ApiScopes)
            {
                configContext.ApiScopes.Add(apiScope.ToEntity());
            }
            await configContext.SaveChangesAsync();
        }

        if (!await configContext.ApiResources.AnyAsync())
        {
            foreach (var apiResource in IdentityServer.Config.ApiResources)
            {
                configContext.ApiResources.Add(apiResource.ToEntity());
            }
            await configContext.SaveChangesAsync();
        }
        
        // Seed admin user
        var userManager = scopedServices.GetRequiredService<UserManager<IdentityApplicationUser>>();
        
        // بررسی با username
        var adminUser = await userManager.FindByNameAsync("09124607630");
        
        if (adminUser == null)
        {
            adminUser = new IdentityApplicationUser
            {
                UserName = "09124607630",
                PhoneNumber = "09124607630",
                PhoneNumberConfirmed = true,
                IsMobileVerified = true,
                IsActive = true,
                FirstName = "مدیر",
                LastName = "سیستم",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true
            };
            
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                Log.Information("کاربر ادمین پیش‌فرض ایجاد شد: {UserId}, {UserName}", adminUser.Id, adminUser.UserName);
            }
            else
            {
                Log.Error("خطا در ایجاد کاربر ادمین: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            // بازنشانی رمز عبور کاربر ادمین
            var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            var resetResult = await userManager.ResetPasswordAsync(adminUser, token, adminPassword);
            
            if (resetResult.Succeeded)
            {
                Log.Information("رمز عبور کاربر ادمین بازنشانی شد: {UserId}, {UserName}", adminUser.Id, adminUser.UserName);
            }
            else
            {
                Log.Error("خطا در بازنشانی رمز عبور: {Errors}", string.Join(", ", resetResult.Errors.Select(e => e.Description)));
            }
            
            // بازنشانی قفل حساب
            await userManager.SetLockoutEndDateAsync(adminUser, null);
            await userManager.ResetAccessFailedCountAsync(adminUser);
            
            // اطمینان از فعال بودن کاربر
            adminUser.IsActive = true;
            await userManager.UpdateAsync(adminUser);
            
            Log.Information("کاربر ادمین از قبل وجود دارد: {UserId}, {UserName}, فعال: {IsActive}", 
                adminUser.Id, adminUser.UserName, adminUser.IsActive);
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "خطا در راه‌اندازی اولیه دیتابیس");
        throw;
    }
}
