using MediatR;

namespace OnlineExamSystem.Application.Features.Exams.Commands.ToggleInvitation
{
    public record ToggleInvitationCommand(int InvitationId, bool IsActive) : IRequest<bool>;
}