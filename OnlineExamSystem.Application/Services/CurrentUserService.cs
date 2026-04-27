using Microsoft.AspNetCore.Http;
using OnlineExamSystem.Infrastructure.Services;
using System.Security.Claims;

namespace OnlineExamSystem.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public CurrentUserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetUserId()
        {
            return _httpContext.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
        }
    }
}