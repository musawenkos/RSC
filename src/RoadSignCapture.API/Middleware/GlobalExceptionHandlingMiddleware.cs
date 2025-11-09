namespace RoadSignCapture.API.Middleware
{
    using System.Net;
    using System.Text.Json;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }
    
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred : {Message}", ex.Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "An unexpected error occurred.",
                    Detail = ex.Message
                };
                var jsonResponse = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}