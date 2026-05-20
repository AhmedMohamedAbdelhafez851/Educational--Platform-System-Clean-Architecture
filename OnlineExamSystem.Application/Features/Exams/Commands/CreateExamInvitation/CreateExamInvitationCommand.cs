using MediatR;
using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Application.Features.Exams.Commands.CreateExamInvitation
{
    public record CreateExamInvitationCommand(int ExamId, DateTime? ExpiresAt, int MaxAttempts = 1) : IRequest<ExamInvitationDto>;
}