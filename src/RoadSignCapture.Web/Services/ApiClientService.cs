using System.Net.Http.Headers;

namespace RoadSignCapture.Web.Services
{
    public interface IApiClientService
    {
        Task<HttpResponseMessage> GetAsync(string endpoint);
        Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content);
        Task<HttpResponseMessage> DeleteAsync(string endpoint);
    }

    public class ApiClientService : IApiClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiClientService> _logger;
        private readonly string _internalToken;

        public ApiClientService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<ApiClientService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Set base address from configuration
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5040";
            _httpClient.BaseAddress = new Uri(apiBaseUrl);

            // Get internal token from configuration
            _internalToken = configuration["ApiSettings:InternalAppToken"] ?? string.Empty;

            if (string.IsNullOrEmpty(_internalToken))
            {
                _logger.LogError("Internal App Token is not configured in ApiSettings:InternalAppToken");
            }

            // Set default headers for all requests
            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            // Add internal token header for authentication with API
            if (!string.IsNullOrEmpty(_internalToken))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Internal-App", _internalToken);
                _logger.LogInformation("Internal App Token configured for API requests");
            }

            // Add other default headers
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // Set timeout
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            try
            {
                _logger.LogInformation("GET request to API: {Endpoint}", endpoint);
                var response = await _httpClient.GetAsync(endpoint);
                LogResponse(response, endpoint);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
        {
            try
            {
                _logger.LogInformation("POST request to API: {Endpoint}", endpoint);
                var response = await _httpClient.PostAsync(endpoint, content);
                LogResponse(response, endpoint);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling POST {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content)
        {
            try
            {
                _logger.LogInformation("PUT request to API: {Endpoint}", endpoint);
                var response = await _httpClient.PutAsync(endpoint, content);
                LogResponse(response, endpoint);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling PUT {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            try
            {
                _logger.LogInformation("DELETE request to API: {Endpoint}", endpoint);
                var response = await _httpClient.DeleteAsync(endpoint);
                LogResponse(response, endpoint);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
                throw;
            }
        }

        private void LogResponse(HttpResponseMessage response, string endpoint)
        {
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "API call successful: {Endpoint} - Status: {StatusCode}", 
                    endpoint, 
                    response.StatusCode);
            }
            else
            {
                _logger.LogWarning(
                    "API call failed: {Endpoint} - Status: {StatusCode}", 
                    endpoint, 
                    response.StatusCode);
            }
        }
    }
}