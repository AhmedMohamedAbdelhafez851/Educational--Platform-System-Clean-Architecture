using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Application.Features.Exams.Commands.CreateExam;
using OnlineExamSystem.Application.Features.Exams.Commands.CreateExamInvitation;
using OnlineExamSystem.Application.Features.Exams.Commands.DeleteExam;
using OnlineExamSystem.Application.Features.Exams.Commands.ToggleInvitation;
using OnlineExamSystem.Application.Features.Exams.Commands.UpdateExam;
using OnlineExamSystem.Application.Features.Exams.Queries.GetAllExams;
using OnlineExamSystem.Application.Features.Exams.Queries.GetExamById;
using OnlineExamSystem.Application.Features.Exams.Queries.GetExamInvitations;

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

        // =====================================================
        // INDEX
        // =====================================================

        public async Task<IActionResult> Index()
        {
            var exams = await _mediator.Send(new GetAllExamsQuery());
            return View(exams);
        }

        // =====================================================
        // CREATE
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Create(CreateExamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mediator.Send(new CreateExamCommand(dto));
            return Ok();
        }

        // =====================================================
        // EDIT
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateExamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mediator.Send(new UpdateExamCommand(id, dto));
            return Ok();
        }

        // =====================================================
        // DELETE
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteExamCommand(id));
            return Ok();
        }

        // =====================================================
        // INVITATIONS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Invitations(int examId)
        {
            var invitations = await _mediator.Send(new GetExamInvitationsQuery(examId));
            ViewBag.ExamId = examId;
            return View(invitations);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvitation(int examId, DateTime? expiresAt, int maxAttempts = 1)
        {
            var invitation = await _mediator.Send(new CreateExamInvitationCommand(examId, expiresAt, maxAttempts));
            return Ok(invitation);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleInvitation(int invitationId, bool isActive)
        {
            var result = await _mediator.Send(new ToggleInvitationCommand(invitationId, isActive));
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> InvitationAttempts(int id)
        {
            var invitations = await _mediator.Send(new GetExamInvitationsQuery(0)); // You need to get specific invitation
            var invitation = invitations.FirstOrDefault(i => i.Id == id);
            if (invitation == null) return NotFound();

            return View(invitation.Attempts);
        }
    }
}