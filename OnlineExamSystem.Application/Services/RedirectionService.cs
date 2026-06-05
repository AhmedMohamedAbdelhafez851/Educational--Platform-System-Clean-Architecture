using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;
using System.Security.Claims;

namespace OnlineExamSystem.Application.Services
{
    public class RedirectionService : IRedirectionService
    {
        private readonly IAuthService _authService;

        public RedirectionService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> RedirectToDashboardAsync(ClaimsPrincipal user)
        {
            var currentUser = await _authService.GetCurrentUserAsync(user);
            if (currentUser == null)
            {
                return new RedirectToActionResult("Login", "Account", null);
            }

            var roles = await _authService.GetUserRolesAsync(currentUser);

            // Role-based redirection (priority order)
            if (roles.Contains("SuperAdmin") || roles.Contains("Admin"))
            {
                return new RedirectToActionResult("Index", "Dashboard", null);
            }

            if (roles.Contains("Teacher"))
            {
                return new RedirectToActionResult("Index", "Exam", null);
            }

            if (roles.Contains("Student"))
            {
                return new RedirectToActionResult("Index", "UserExam", null);
            }

            return new RedirectToActionResult("Index", "Home", null);
        }

        public IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/"))
            {
                return new LocalRedirectResult(returnUrl);
            }
            return new RedirectToActionResult("Index", "Home", null);
        }
    }
}