using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IRedirectionService
    {
        Task<IActionResult> RedirectToDashboardAsync(ClaimsPrincipal user);
        IActionResult RedirectToLocal(string returnUrl);
    }
}