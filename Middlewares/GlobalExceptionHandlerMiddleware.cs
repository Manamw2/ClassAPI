namespace ClassAPI.Middlewares
{
    using global::ClassAPI.Common;
    using System.Net;
    using System.Text.Json;

    namespace ClassAPI.Middleware
    {
        public class GlobalExceptionMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<GlobalExceptionMiddleware> _logger;

            public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception");

                    var response = new GlobalResponse<string>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred.",
                        Data = ex.Message
                    };

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(response);
                }
            }
        }
    }
}
