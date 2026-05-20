using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Application.Features.Exams.Commands.JoinExamByInvitation;
using OnlineExamSystem.Application.Features.Exams.Queries.GetExamInvitations;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Web.Controllers
{
    public class UserExamController : Controller
    {
        private readonly IExamService _examService;
        private readonly IExamSubmissionService _submissionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public UserExamController(
            IExamService examService,
            IExamSubmissionService submissionService,
            UserManager<ApplicationUser> userManager,
            IMediator mediator,
            IUnitOfWork unitOfWork)
        {
            _examService = examService;
            _submissionService = submissionService;
            _userManager = userManager;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Join(string? code)
        {
            var model = new JoinExamDto
            {
                InvitationCode = code ?? string.Empty
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Join(JoinExamDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _mediator.Send(new JoinExamByInvitationCommand(model));

            if (result.Success)
            {
                TempData["AttemptId"] = result.AttemptId;
                TempData["StudentName"] = model.StudentName;
                TempData["InvitationCode"] = model.InvitationCode;
                TempData["StudentEmail"] = model.StudentEmail;
                TempData["StudentId"] = model.StudentId;

                if (!string.IsNullOrEmpty(result.RedirectUrl))
                {
                    return Redirect(result.RedirectUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid redirect URL");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "Failed to join exam");
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> TakeExam(int id, int? attemptId)
        {
            var exam = await _examService.GetExamForTakingAsync(id);
            if (exam == null)
            {
                return NotFound();
            }

            if (attemptId.HasValue)
            {
                ViewBag.AttemptId = attemptId;
                ViewBag.StudentName = TempData["StudentName"] as string ?? string.Empty;
                ViewBag.StudentEmail = TempData["StudentEmail"] as string ?? string.Empty;
                ViewBag.StudentId = TempData["StudentId"] as string ?? string.Empty;
            }

            return View(exam);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitExam(int examId, Dictionary<int, int> answers, int? attemptId)
        {
            try
            {
                string? userId = null;
                string studentName = "Anonymous";
                string studentEmail = "";
                string studentId = "";

                // Get questions for this exam
                var questions = await _unitOfWork.Repository<Question>()
                    .GetQueryable()
                    .Where(q => q.ExamId == examId)
                    .Include(q => q.Choices)
                    .ToListAsync();

                int totalQuestions = questions.Count;
                int correctAnswersCount = 0;

                foreach (var question in questions)
                {
                    if (answers.ContainsKey(question.QuestionId))
                    {
                        var selectedChoiceId = answers[question.QuestionId];
                        if (selectedChoiceId == question.CorrectChoiceId)
                        {
                            correctAnswersCount++;
                        }
                    }
                }

                double score = totalQuestions > 0 ? (double)correctAnswersCount / totalQuestions * 100 : 0;
                bool isPassed = score >= 50;

                // Handle authenticated user
                if (User.Identity?.IsAuthenticated == true)
                {
                    var authenticatedUserId = _userManager.GetUserId(User);
                    if (!string.IsNullOrEmpty(authenticatedUserId))
                    {
                        var user = await _userManager.FindByIdAsync(authenticatedUserId);
                        if (user != null)
                        {
                            userId = authenticatedUserId;
                            studentName = user.FullName ?? user.UserName ?? authenticatedUserId;
                            studentEmail = user.Email ?? "";
                        }
                    }
                }
                // Handle anonymous user via invitation
                else if (attemptId.HasValue)
                {
                    var attempt = await _mediator.Send(new GetInvitationAttemptQuery(attemptId.Value));
                    studentName = attempt?.StudentName ?? TempData["StudentName"] as string ?? "Anonymous Student";
                    studentEmail = attempt?.StudentEmail ?? TempData["StudentEmail"] as string ?? "";
                    studentId = attempt?.StudentId ?? TempData["StudentId"] as string ?? "";
                    userId = null; // Keep null for anonymous users
                }
                else
                {
                    return Json(new { success = false, message = "Please use a valid invitation link." });
                }

                // Create submission
                var submission = new ExamSubmission
                {
                    UserId = userId,
                    ExamId = examId,
                    SubmissionDate = DateTime.UtcNow,
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswersCount,
                    Score = score,
                    IsPassed = isPassed,
                    StudentName = studentName,
                    StudentEmail = studentEmail,
                    StudentId = studentId,
                    Answers = new List<UserAnswer>()
                };

                // Save answers
                foreach (var answer in answers)
                {
                    submission.Answers.Add(new UserAnswer
                    {
                        QuestionId = answer.Key,
                        SelectedChoiceId = answer.Value
                    });
                }

                await _unitOfWork.Repository<ExamSubmission>().AddAsync(submission);
                await _unitOfWork.SaveChangesAsync();

                // Update invitation attempt
                if (attemptId.HasValue)
                {
                    await _mediator.Send(new UpdateInvitationAttemptCommand(attemptId.Value, submission.SubmissionId));
                }

                return Json(new { success = true, submissionId = submission.SubmissionId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ViewResult(int id)
        {
            var submission = await _submissionService.GetSubmissionDetailsAsync(id);
            if (submission != null)
            {
                if (string.IsNullOrEmpty(submission.UserId) && !string.IsNullOrEmpty(submission.StudentName))
                {
                    ViewBag.StudentName = submission.StudentName;
                }
                return View(submission);
            }

            var attempt = await _mediator.Send(new GetInvitationAttemptQuery(id));
            if (attempt != null && attempt.SubmissionId.HasValue)
            {
                submission = await _submissionService.GetSubmissionDetailsAsync(attempt.SubmissionId.Value);
                if (submission != null)
                {
                    ViewBag.StudentName = attempt.StudentName;
                    return View(submission);
                }
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                if (!string.IsNullOrEmpty(userId))
                {
                    submission = await _submissionService.GetSubmissionDetailsAsync(id, userId);
                    if (submission != null)
                    {
                        return View(submission);
                    }
                }
            }

            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var exams = await _examService.GetAllExamsAsync();
            var submissions = await _submissionService.GetUserSubmissionsAsync(userId);

            var submissionsGrouped = submissions
                .GroupBy(s => s.ExamId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(s => s.SubmissionDate).ToList());

            ViewBag.SubmissionsGrouped = submissionsGrouped;
            return View(exams);
        }
    }
}