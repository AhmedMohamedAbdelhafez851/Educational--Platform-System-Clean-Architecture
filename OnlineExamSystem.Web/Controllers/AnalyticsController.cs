using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Features.Analytics.Queries.GetExamAnalytics;
using OnlineExamSystem.Application.Features.Exams.Queries.GetAllExams;

namespace OnlineExamSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class AnalyticsController : Controller
    {
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Overview page - Shows all exams for selection
        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var exams = await _mediator.Send(new GetAllExamsQuery());
            return View(exams);
        }

        // Detailed report for specific exam
        [HttpGet]
        public async Task<IActionResult> ExamReport(int examId)
        {
            var analytics = await _mediator.Send(new GetExamAnalyticsQuery(examId));
            if (analytics == null)
            {
                return NotFound();
            }
            return View(analytics);
        }
    }
}