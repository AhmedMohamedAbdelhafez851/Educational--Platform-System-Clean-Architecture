using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Web.Controllers
{
    public class ExamController : Controller
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _examService.GetAllExamsAsync();
            return View(exams);
        }

        public IActionResult Create()
        {
            return View(new CreateExamDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExamDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _examService.CreateExamAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null) return NotFound();

            return View(exam);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateExamDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _examService.EditExamAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _examService.DeleteExamAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}