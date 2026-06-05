using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Web.ViewModels.UserDTO;

namespace OnlineExamSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRedirectionService _redirectionService;
        private readonly ILanguageService _languageService;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(
            IAuthService authService,
            IRedirectionService redirectionService,
            ILanguageService languageService,
            IStringLocalizer<AccountController> localizer)
        {
            _authService = authService;
            _redirectionService = redirectionService;
            _languageService = languageService;
            _localizer = localizer;
        }

        // =====================================================
        // LOGIN
        // =====================================================

        [HttpGet]
        public IActionResult Login(string returnUrl = null!)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null!)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.FindUserByEmailAsync(model.Email);

            if (user == null)
            {
                SetLoginError();
                return View(model);
            }

            if (await _authService.IsUserLockedOutAsync(user))
            {
                var remainingSeconds = await _authService.GetLockoutRemainingSecondsAsync(user);
                SetLockoutError(remainingSeconds ?? 60);
                return View(model);
            }

            var result = await _authService.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                return await RedirectToDashboardAsync();
            }

            if (result.IsLockedOut)
            {
                SetLockoutError(60);
                return View(model);
            }

            SetLoginError();
            return View(model);
        }

        // =====================================================
        // REGISTER
        // =====================================================

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _authService.CreateUserAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _authService.SignInAsync(user, isPersistent: false);
            return await RedirectToDashboardAsync();
        }

        // =====================================================
        // LOGOUT
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // =====================================================
        // LANGUAGE
        // =====================================================

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            _languageService.SetLanguageCookie(HttpContext, culture);
            return _languageService.HandleLanguageChange(culture, returnUrl, RedirectToDashboard);
        }

        // =====================================================
        // OTHER
        // =====================================================

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null!)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // =====================================================
        // PRIVATE HELPERS
        // =====================================================

        private async Task<IActionResult> RedirectToDashboardAsync()
        {
            return await _redirectionService.RedirectToDashboardAsync(User);
        }

        private IActionResult RedirectToDashboard()
        {
            return RedirectToDashboardAsync().GetAwaiter().GetResult();
        }

        private void SetLoginError()
        {
            ViewBag.InvalidLogin = true;
        }

        private void SetLockoutError(int seconds)
        {
            ViewBag.LockedOut = true;
            ViewBag.LockoutSeconds = seconds;
        }
    }
}