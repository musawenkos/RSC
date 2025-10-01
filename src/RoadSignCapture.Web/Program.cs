using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Infrastructure.Data;
using RoadSignCapture.Infrastructure.Services;
using System.Security.Claims;

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

    // CRITICAL: This tells ASP.NET Core to use the original host for redirects
    options.ForwardLimit = null;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RSCDbContext>(options =>
    options.UseSqlServer(connectionString));

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ??
                   builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);

        // CRITICAL: Remove SignedOutCallbackPath to prevent loops
        options.SignedOutCallbackPath = "/signout-callback-oidc";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("User.Read");
        options.Scope.Add("Files.ReadWrite");
        options.SaveTokens = true;

        // Disable HTTPS requirement if behind proxy handling SSL
        options.RequireHttpsMetadata = false;

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async context =>
            {
                try
                {
                    var userEmail = context.Principal?.FindFirst(ClaimTypes.Email)?.Value ??
                                   context.Principal?.FindFirst("preferred_username")?.Value ??
                                   context.Principal?.FindFirst("upn")?.Value;

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
                // CRITICAL: Ensure correct redirect URI is used
                var request = context.HttpContext.Request;
                var redirectUri = $"{request.Scheme}://{request.Host}{options.CallbackPath}";
                context.ProtocolMessage.RedirectUri = redirectUri;

                if (context.Properties.Items.ContainsKey("scopes"))
                {
                    context.ProtocolMessage.Prompt = "consent";
                }
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
            },

            OnMessageReceived = context =>
            {
                // Log for debugging
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Message received. Request path: {Path}", context.Request.Path);
                return Task.CompletedTask;
            }
        };
    })
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    // CRITICAL: Don't use FallbackPolicy if it's causing issues
    // Comment this out temporarily to test
    // options.FallbackPolicy = options.DefaultPolicy;

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

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Users", "RequireUser");
})
.AddMicrosoftIdentityUI();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRole, UserRoleService>();

// Add logging for debugging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// CRITICAL: UseForwardedHeaders MUST be first
app.UseForwardedHeaders();

// CRITICAL: Set the path base for apps hosted in subdirectories
var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

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

// CRITICAL: Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Add middleware to log requests for debugging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path} {Scheme} {Host}",
        context.Request.Method,
        context.Request.Path,
        context.Request.Scheme,
        context.Request.Host);
    await next();
});

app.MapRazorPages();
app.MapControllers();

app.Run();