using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Application.Features.Exams.Commands.CreateExam;
using OnlineExamSystem.Application.Features.Exams.Commands.DeleteExam;
using OnlineExamSystem.Application.Features.Exams.Commands.UpdateExam;
using OnlineExamSystem.Application.Features.Exams.Queries.GetAllExams;

namespace OnlineExamSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class ExamController : Controller
    {
        private readonly IMediator _mediator;

        public ExamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _mediator.Send(new GetAllExamsQuery());
            return View(exams);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mediator.Send(new CreateExamCommand(dto));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateExamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mediator.Send(new UpdateExamCommand(id, dto));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteExamCommand(id));
            return Ok();
        }
    }
}