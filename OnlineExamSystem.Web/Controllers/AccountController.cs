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
            // ✅ FIX: If user is already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin") || User.IsInRole("Teacher"))
                {
                    return RedirectToAction("Index", "Exam");
                }
                else
                {
                    return RedirectToAction("Index", "UserExam");
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null!)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, _localizer["EmailNotFound"]);
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Redirect to dashboard based on role
                    if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Teacher"))
                    {
                        return RedirectToAction("Index", "Exam");
                    }
                    else
                    {
                        return RedirectToAction("Index", "UserExam");
                    }
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, _localizer["AccountLocked"]);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer["InvalidCredentials"]);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            // ✅ FIX: If user is already authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin") || User.IsInRole("Teacher"))
                {
                    return RedirectToAction("Index", "Exam");
                }
                else
                {
                    return RedirectToAction("Index", "UserExam");
                }
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

                    // Redirect to dashboard based on role
                    if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Teacher"))
                    {
                        return RedirectToAction("Index", "Exam");
                    }
                    else
                    {
                        return RedirectToAction("Index", "UserExam");
                    }
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
                    if (User.Identity?.IsAuthenticated == true)
                    {
                        var user = _userManager.GetUserAsync(User).Result;
                        if (user != null)
                        {
                            if (_userManager.IsInRoleAsync(user, "Admin").Result || _userManager.IsInRoleAsync(user, "Teacher").Result)
                            {
                                return RedirectToAction("Index", "Exam");
                            }
                            else
                            {
                                return RedirectToAction("Index", "UserExam");
                            }
                        }
                    }
                    return RedirectToAction("Login", "Account");
                }

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Login", "Account");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}