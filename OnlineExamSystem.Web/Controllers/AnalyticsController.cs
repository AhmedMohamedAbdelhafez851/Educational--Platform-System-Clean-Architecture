using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Features.Analytics.Queries.GetExamStatistics;
using OnlineExamSystem.Application.Features.Analytics.Queries.GetQuestionPerformance;
using OnlineExamSystem.Application.Features.Analytics.Queries.GetStudentsPerformance;

namespace OnlineExamSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Teacher")] // Adjust roles as needed
    public class AnalyticsController : Controller
    {
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> ExamReport(int examId)
        {
            // Fetch all data for this exam
            var statistics = await _mediator.Send(new GetExamStatisticsQuery(examId));
            var questions = await _mediator.Send(new GetQuestionPerformanceQuery(examId));
            var students = await _mediator.Send(new GetStudentsPerformanceQuery(examId));

            ViewBag.ExamId = examId;
            ViewBag.Statistics = statistics;
            ViewBag.Questions = questions;
            ViewBag.Students = students;

            return View();
        }
    }
}