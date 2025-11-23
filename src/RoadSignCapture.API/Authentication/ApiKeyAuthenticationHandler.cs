using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public string HeaderName { get; set; } = "X-API-Key";
}

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
        _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        _logger.LogInformation("HandleAuthenticateAsync called for path: {Path}", Request.Path);

        // Check if request is from internal Web app
        var internalToken = Request.Headers["X-Internal-App"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(internalToken))
        {
            _logger.LogInformation("Internal token received: {InternalToken}", internalToken);
            
            
            var expectedInternalToken = _configuration["ApiSettings:InternalAppToken"];
            
            if (string.IsNullOrEmpty(expectedInternalToken))
            {
                _logger.LogError("InternalAppToken not configured in ApiSettings");
                return Task.FromResult(AuthenticateResult.Fail("Server configuration error"));
            }

            if (internalToken == expectedInternalToken)
            {
                _logger.LogInformation("Internal request authenticated successfully");
                var claims = new[] { new Claim(ClaimTypes.Name, "InternalApp") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                _logger.LogWarning("Invalid internal token provided");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Internal Token"));
            }
        }

        // External request - require API key
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var apiKeyHeaderValues))
        {
            _logger.LogWarning("No API Key header found");
            return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            _logger.LogWarning("Empty API Key provided");
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        _logger.LogInformation("API Key provided: {ApiKey}", providedApiKey);

        
        var validApiKeys = _configuration.GetSection("ApiSettings:ApiKeys").Get<string[]>();
        
        if (validApiKeys == null || validApiKeys.Length == 0)
        {
            _logger.LogError("No API keys configured in ApiSettings:ApiKeys");
            return Task.FromResult(AuthenticateResult.Fail("Server configuration error"));
        }

        _logger.LogInformation("Configured API keys count: {Count}", validApiKeys.Length);

        if (!validApiKeys.Contains(providedApiKey))
        {
            _logger.LogWarning("Invalid API Key attempt: {ApiKey}", providedApiKey);
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        _logger.LogInformation("External request authenticated successfully with API Key: {ApiKey}", providedApiKey);

        var externalClaims = new[] 
        { 
            new Claim(ClaimTypes.Name, "ExternalClient"),
            new Claim("ApiKey", providedApiKey)
        };
        var externalIdentity = new ClaimsIdentity(externalClaims, Scheme.Name);
        var externalPrincipal = new ClaimsPrincipal(externalIdentity);
        var externalTicket = new AuthenticationTicket(externalPrincipal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(externalTicket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        _logger.LogWarning("Authentication challenge triggered");
        
        Response.StatusCode = 401;
        Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new 
        { 
            error = "Unauthorized",
            message = "Valid API Key required. Include X-API-Key header."
        });
        return Response.WriteAsync(result);
    }
}