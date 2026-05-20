using MediatR;
using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Application.Features.Exams.Queries.GetExamInvitations
{
    public record GetExamInvitationsQuery(int ExamId) : IRequest<List<ExamInvitationDto>>;
}