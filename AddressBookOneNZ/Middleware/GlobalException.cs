using System.Net;

namespace AddressBookOneNZ.Middleware
{
    public class GlobalException
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalException> _logger;

        public GlobalException(RequestDelegate next, ILogger<GlobalException> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred.");

                // Map exception type -> HTTP Status Code
                var statusCode = ex switch
                {
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    InvalidOperationException => (int)HttpStatusCode.Conflict,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "text/plain";

                //// Show only generic message, not stack trace
                //var message = statusCode switch
                //{
                //    400 => "Bad Request",
                //    404 => "Not Found",
                //    409 => "Conflict",
                //    500 => "Internal Server Error",
                //    _ => "An error occurred"
                //};

                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
