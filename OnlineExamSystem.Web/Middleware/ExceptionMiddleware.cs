using System.Net;

namespace OnlineExamSystem.Web.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                // In development, show the actual error
                if (_env.IsDevelopment())
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(ex.ToString());
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync("Something went wrong. Please try again.");
                }
            }
        }
    }
}