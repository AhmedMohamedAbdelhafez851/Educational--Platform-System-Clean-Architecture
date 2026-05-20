using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.Features.Dashboard.Queries.GetGeneralDashboard;

namespace OnlineExamSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _mediator.Send(new GetGeneralDashboardQuery());
            return View(dashboard);
        }
    }
}