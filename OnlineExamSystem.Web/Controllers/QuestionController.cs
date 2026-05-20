using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Question;

namespace OnlineExamSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task<IActionResult> Index(int examId)
        {
            var questions = await _questionService.GetQuestionsByExamAsync(examId);
            ViewBag.ExamId = examId;
            ViewBag.ExamTitle = await GetExamTitle(examId);
            return View(questions);
        }

        public async Task<IActionResult> Add(int examId)
        {
            var examTitle = await GetExamTitle(examId);
            ViewBag.ExamTitle = examTitle;

            var dto = new CreateQuestionDto
            {
                ExamId = examId,
                CorrectChoiceIndex = -1,
                Choices = new List<ChoiceDto> { new(), new(), new(), new() }
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ExamTitle = await GetExamTitle(dto.ExamId);
                return View(dto);
            }

            await _questionService.AddQuestionAsync(dto);
            TempData["SuccessMessage"] = "Question added successfully!";
            return RedirectToAction(nameof(Index), new { examId = dto.ExamId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _questionService.GetQuestionByIdAsync(id);
            if (dto == null) return NotFound();

            ViewBag.ExamTitle = await GetExamTitle(dto.ExamId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ExamTitle = await GetExamTitle(dto.ExamId);
                return View(dto);
            }

            await _questionService.EditQuestionAsync(dto);
            TempData["SuccessMessage"] = "Question updated successfully!";
            return RedirectToAction(nameof(Index), new { examId = dto.ExamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int examId)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(id);
                return Json(new { success = true, message = "Question deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<string> GetExamTitle(int examId)
        {
            var exam = await _questionService.GetExamByIdAsync(examId);
            return exam?.Title ?? "Exam";
        }
    }
}