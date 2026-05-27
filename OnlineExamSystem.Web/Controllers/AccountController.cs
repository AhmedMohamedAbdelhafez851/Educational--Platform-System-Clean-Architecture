using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Web.ViewModels.UserDTO;

namespace OnlineExamSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<AccountController> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null!)
        {
            // If user is already authenticated, redirect to appropriate dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
     LoginViewModel model,
     string returnUrl = null!)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // =============================================
            // BASIC VALIDATION
            // =============================================

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // =============================================
            // FIND USER
            // =============================================

            var user = await _userManager.FindByEmailAsync(model.Email);

            // Prevent email enumeration attack
            // Don't reveal if email exists or not

            if (user == null)
            {
                ViewBag.InvalidLogin = true;

                return View(model);
            }

            // =============================================
            // CHECK LOCKOUT
            // =============================================

            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd =
                    await _userManager.GetLockoutEndDateAsync(user);

                if (lockoutEnd.HasValue)
                {
                    var remaining =
                        lockoutEnd.Value.UtcDateTime - DateTime.UtcNow;

                    ViewBag.LockedOut = true;

                    ViewBag.LockoutSeconds =
                        Math.Max((int)remaining.TotalSeconds, 0);
                }

                return View(model);
            }

            // =============================================
            // LOGIN ATTEMPT
            // =============================================

            var result =
                await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true
                );

            // =============================================
            // SUCCESS
            // =============================================

            if (result.Succeeded)
            {
                return RedirectToDashboard();
            }

            // =============================================
            // LOCKED
            // =============================================

            if (result.IsLockedOut)
            {
                ViewBag.LockedOut = true;

                ViewBag.LockoutSeconds = 60;

                return View(model);
            }

            // =============================================
            // INVALID LOGIN
            // =============================================

            ViewBag.InvalidLogin = true;

            return View(model);
        }

        // ✅ Helper method to redirect users based on their role
        private IActionResult RedirectToDashboard()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var roles = _userManager.GetRolesAsync(user).Result;

            // SuperAdmin -> Dashboard (General Dashboard)
            if (roles.Contains("SuperAdmin"))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Admin -> Dashboard (General Dashboard)
            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Teacher -> Exam Management
            if (roles.Contains("Teacher"))
            {
                return RedirectToAction("Index", "Exam");
            }

            // Student -> User Exam (Take exams)
            if (roles.Contains("Student"))
            {
                return RedirectToAction("Index", "UserExam");
            }

            // Default fallback
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            // If user is already authenticated, redirect to dashboard
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
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.CapturedImageData))
                    {
                        var base64Data = model.CapturedImageData.Split(',')[1];
                        var imageBytes = Convert.FromBase64String(base64Data);
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", $"{user.Id}.png");

                        Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);
                        await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect based on role after registration
                    return RedirectToDashboard();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null!)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            try
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        IsEssential = true,
                        HttpOnly = true
                    }
                );

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToDashboard();
                }

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToDashboard();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}