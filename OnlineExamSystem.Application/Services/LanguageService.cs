using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Abstraction;

namespace OnlineExamSystem.Application.Services
{
    public class LanguageService : ILanguageService
    {
        public void SetLanguageCookie(HttpContext httpContext, string culture)
        {
            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = true
                }
            );
        }

        public IActionResult HandleLanguageChange(string culture, string returnUrl, Func<IActionResult> redirectToDashboard)
        {
            try
            {
                if (string.IsNullOrEmpty(returnUrl) || !returnUrl.StartsWith("/"))
                {
                    return redirectToDashboard();
                }

                return new LocalRedirectResult(returnUrl);
            }
            catch
            {
                return new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}