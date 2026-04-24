using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Question;

namespace OnlineExamSystem.Web.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IExamService _examService;

        public QuestionController(
            IQuestionService questionService,
            IExamService examService)
        {
            _questionService = questionService;
            _examService = examService;
        }

        public async Task<IActionResult> Index(int examId)
        {
            var questions = await _questionService.GetQuestionsByExamAsync(examId);

            ViewBag.ExamId = examId;
            ViewBag.ExamTitle = "Exam Questions";

            return View(questions);
        }

        public IActionResult Add(int examId)
        {
            var dto = new CreateQuestionDto
            {
                ExamId = examId,
                CorrectChoiceIndex = -1,
                Choices = new List<ChoiceDto>
                {
                    new(), new(), new(), new()
                }
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _questionService.AddQuestionAsync(dto);

            return RedirectToAction(nameof(Index), new { examId = dto.ExamId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _questionService.GetQuestionByIdAsync(id);

            if (dto == null)
                return NotFound();

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _questionService.EditQuestionAsync(dto);

            return RedirectToAction(nameof(Index), new { examId = dto.ExamId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int examId)
        {
            await _questionService.DeleteQuestionAsync(id);

            return RedirectToAction(nameof(Index), new { examId });
        }
    }
}