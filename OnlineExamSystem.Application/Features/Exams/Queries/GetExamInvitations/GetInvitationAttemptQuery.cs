using MediatR;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Queries.GetExamInvitations
{
    public record GetInvitationAttemptQuery(int AttemptId) : IRequest<ExamInvitationAttempt?>;
}