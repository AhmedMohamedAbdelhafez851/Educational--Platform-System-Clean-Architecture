using MediatR;

namespace OnlineExamSystem.Application.Features.Exams.Commands.JoinExamByInvitation
{
    public record UpdateInvitationAttemptCommand(int AttemptId, int SubmissionId) : IRequest<bool>;
}