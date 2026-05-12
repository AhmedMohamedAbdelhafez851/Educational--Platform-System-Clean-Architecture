using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Web.Controllers
{
    [Authorize]
    public class UserExamController : Controller
    {
        private readonly IExamService _examService;
        private readonly IExamSubmissionService _submissionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserExamController(IExamService examService, IExamSubmissionService submissionService, UserManager<ApplicationUser> userManager)
        {
            _examService = examService;
            _submissionService = submissionService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var exams = await _examService.GetAllExamsAsync();
            var submissions = await _submissionService.GetUserSubmissionsAsync(userId);

            var submissionsGrouped = submissions
                .GroupBy(s => s.ExamId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(s => s.SubmissionDate).ToList());

            ViewBag.SubmissionsGrouped = submissionsGrouped;
            return View(exams);
        }

        public async Task<IActionResult> TakeExam(int id)
        {
            var exam = await _examService.GetExamForTakingAsync(id);
            if (exam == null) return NotFound();
            return View(exam);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(int examId, Dictionary<int, int> answers)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId)) return Json(new { success = false, message = "User not authenticated." });

                var submission = await _submissionService.SubmitExamAsync(userId, examId, answers);
                return Json(new { success = true, submissionId = submission.SubmissionId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> ViewResult(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var submission = await _submissionService.GetSubmissionDetailsAsync(id, userId);
            if (submission == null) return NotFound();

            return View(submission);
        }
    }
}