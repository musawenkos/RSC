using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using RoadSignCapture.Infrastructure.Data;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Users.Commands;
using RoadSignCapture.Infrastructure.Services;
using System.Security.Claims;
using RoadSignCapture.Core.Projects.Commands;

var builder = WebApplication.CreateBuilder(args);

// IMPORTANT: Configure forwarded headers FIRST before any authentication
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                              ForwardedHeaders.XForwardedProto |
                              ForwardedHeaders.XForwardedHost;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();

    options.ForwardedProtoHeaderName = "X-Forwarded-Proto";
    options.ForwardedForHeaderName = "X-Forwarded-For";
    options.ForwardedHostHeaderName = "X-Forwarded-Host";

    options.ForwardLimit = null;
});

builder.Services.AddDbContext<RSCDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions
            .EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)
    ));

// Configure Auth0 Authentication
builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["AzureAd:Domain"];
        options.ClientId = builder.Configuration["AzureAd:ClientId"];
        options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
        options.Scope = "openid profile email";

        // Callback paths
        options.CallbackPath = "/callback";
        //options.SignOutScheme = "/signout-callback";

        // Configure OpenIdConnect options
        options.OpenIdConnectEvents = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
        {
            OnTokenValidated = async context =>
            {
                try
                {
                    var userEmail = context.Principal?.FindFirst(ClaimTypes.Email)?.Value ??
                                   context.Principal?.FindFirst("email")?.Value;

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var userRoleService = context.HttpContext.RequestServices
                            .GetRequiredService<IUserRole>();

                        var roles = await userRoleService.GetUserRolesAsync(userEmail);

                        var identity = (ClaimsIdentity)context.Principal!.Identity!;
                        foreach (var role in roles)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error in OnTokenValidated event");
                }
            },

            OnRedirectToIdentityProvider = context =>
            {
                var request = context.HttpContext.Request;
                var pathBase = context.HttpContext.Request.PathBase.Value ?? string.Empty;

                var redirectUri = $"{request.Scheme}://{request.Host}/callback";
                context.ProtocolMessage.RedirectUri = redirectUri;

                return Task.CompletedTask;
            },

            OnRemoteFailure = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Failure, "Remote authentication failure");

                context.Response.Redirect("/Error");
                context.HandleResponse();
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Authentication failed");

                context.Response.Redirect("/Error");
                context.HandleResponse();
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUser", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Viewer", "Client", "Designer", "SysAdmin"));

    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("SysAdmin"));

    options.AddPolicy("RequireDesigner", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Designer", "SysAdmin"));

    options.AddPolicy("RequireClient", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Client", "SysAdmin"));
});

builder.Services.AddControllers();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Users", "RequireUser");
});

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRole, UserRoleService>();
builder.Services.AddScoped<IProjectService,ProjectService>();

builder.Services.AddScoped<UserHandler>();
builder.Services.AddScoped<ProjectHandler>();

builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// CRITICAL: UseForwardedHeaders MUST be first
app.UseForwardedHeaders();

await MigrateDatabaseAsync(app);

// CRITICAL: Set the path base for apps hosted in subdirectories

//app.UsePathBase("/RoadSignCapture/");
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

/*app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path} {Scheme} {Host}",
        context.Request.Method,
        context.Request.Path,
        context.Request.Scheme,
        context.Request.Host);
    await next();
});*/

app.MapRazorPages();
app.MapControllers();

app.Run();

// <summary>
/// Automatically applies pending migrations when the application starts
/// </summary>
static async Task MigrateDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<RSCDbContext>();
        
        logger.LogInformation("Starting database migration...");
        
        // Check if database exists
        var canConnect = await context.Database.CanConnectAsync();
        if (!canConnect)
        {
            logger.LogWarning("Cannot connect to database. Waiting for database to be available...");
            
            // Retry logic
            var maxRetries = 10;
            var retryCount = 0;
            
            while (!canConnect && retryCount < maxRetries)
            {
                await Task.Delay(5000); // Wait 5 seconds
                canConnect = await context.Database.CanConnectAsync();
                retryCount++;
                logger.LogInformation($"Retry {retryCount}/{maxRetries}...");
            }
            
            if (!canConnect)
            {
                throw new Exception("Could not connect to database after multiple");
            }
        }
        
        // Get pending migrations
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        
        if (pendingMigrations.Any())
        {
            logger.LogInformation($"Found {pendingMigrations.Count()} pending migrations:");
            foreach (var migration in pendingMigrations)
            {
                logger.LogInformation($"  - {migration}");
            }
            
            // Apply migrations
            logger.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync();
            
            logger.LogInformation("✅ Database migration completed successfully");
        }
        else
        {
            logger.LogInformation("Database is already up to date");
        }
        
        // Log applied migrations
        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
        logger.LogInformation($"Total applied migrations: {appliedMigrations.Count()}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred while migrating the database");
        
        // Decide whether to throw or continue
        if (app.Environment.IsProduction())
        {
            // In production, you might want to fail fast
            throw;
        }
        else
        {
            // In development, you might want to continue
            logger.LogWarning("Continuing despite migration error (Development mode)");
        }
    }
}