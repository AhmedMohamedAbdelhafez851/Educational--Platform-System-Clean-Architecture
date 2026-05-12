using Microsoft.AspNetCore.Http;
using OnlineExamSystem.Application.Abstraction;
using System.Security.Claims;

namespace OnlineExamSystem.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId() => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        public string GetUserName() => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Anonymous";

        public bool IsAuthenticated() => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}