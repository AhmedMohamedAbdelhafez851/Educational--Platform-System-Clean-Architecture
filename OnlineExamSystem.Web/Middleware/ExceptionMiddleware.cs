using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next,
                               ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (context.Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                await context.Response.WriteAsync("API Error occurred");
            }
            else
            {
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}