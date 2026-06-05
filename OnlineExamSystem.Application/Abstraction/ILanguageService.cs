using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface ILanguageService
    {
        void SetLanguageCookie(HttpContext httpContext, string culture);
        IActionResult HandleLanguageChange(string culture, string returnUrl, Func<IActionResult> redirectToDashboard);
    }
}