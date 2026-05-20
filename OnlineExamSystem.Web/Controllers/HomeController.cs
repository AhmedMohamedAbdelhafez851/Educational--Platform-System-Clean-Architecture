using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Web.Models;
using System.Diagnostics;

namespace OnlineExamSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // If user is authenticated, redirect to appropriate dashboard
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

            // Otherwise redirect to login page
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}