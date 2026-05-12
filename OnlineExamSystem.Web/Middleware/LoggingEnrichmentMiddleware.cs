using Serilog.Context;

namespace OnlineExamSystem.Web.Middleware
{
    public class LoggingEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userId = context.User?.Identity?.IsAuthenticated == true
                ? context.User.FindFirst("sub")?.Value ?? context.User.Identity?.Name
                : "Anonymous";

            var ip = context.Connection.RemoteIpAddress?.ToString();

            using (LogContext.PushProperty("User", userId))
            using (LogContext.PushProperty("IP", ip))
            {
                await _next(context);
            }
        }
    }
}