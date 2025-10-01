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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                              ForwardedHeaders.XForwardedProto |
                              ForwardedHeaders.XForwardedHost;

    // Trust all proxies - adjust this for your security requirements
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();

    // Optional: specify the header names if they differ
    options.ForwardedProtoHeaderName = "X-Forwarded-Proto";
    options.ForwardedForHeaderName = "X-Forwarded-For";
    options.ForwardedHostHeaderName = "X-Forwarded-Host";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RSCDbContext>(options =>
    options.UseSqlServer(connectionString));

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("User.Read");
        options.Scope.Add("Files.ReadWrite");
        options.SaveTokens = true;

        options.Events.OnTokenValidated = async context =>
        {
            try
            {
                // Get the user's email from the token
                var userEmail = context.Principal?.FindFirst(ClaimTypes.Email)?.Value ??
                               context.Principal?.FindFirst("preferred_username")?.Value ??
                               context.Principal?.FindFirst("upn")?.Value;

                if (!string.IsNullOrEmpty(userEmail))
                {
                    // Get the user role service from DI container
                    var userRoleService = context.HttpContext.RequestServices
                        .GetRequiredService<IUserRole>();

                    // Retrieve user roles from your database
                    var roles = await userRoleService.GetUserRolesAsync(userEmail);

                    // Add role claims to the user's identity
                    var identity = (ClaimsIdentity)context.Principal!.Identity!;
                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't fail authentication
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error in OnTokenValidated event");
            }
            // This ensures the user has consented to the required scopes
            options.Events.OnRedirectToIdentityProvider = context =>
            {
                // Only add consent prompt if we're requesting additional scopes
                if (context.Properties.Items.ContainsKey("scopes"))
                {
                    context.ProtocolMessage.Prompt = "consent";
                }
                return Task.CompletedTask;
            };

        };

    })
        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;

    // Role-based policies
    options.AddPolicy("RequireUser", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Viewer", "Client", "Designer", "SysAdmin"));
    //Admin only
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("SysAdmin"));
    // Designer or higher
    options.AddPolicy("RequireDesigner", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Designer", "SysAdmin"));
    // Client or higher
    options.AddPolicy("RequireClient", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Client", "SysAdmin"));

});

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // Apply policies to specific pages/folders
    options.Conventions.AuthorizePage("/Users", "RequireUser");
})
    .AddMicrosoftIdentityUI();
builder.Services.AddScoped<ICompanyService,CompanyService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRole, UserRoleService>();

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    //app.UseHttpsRedirection();
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();