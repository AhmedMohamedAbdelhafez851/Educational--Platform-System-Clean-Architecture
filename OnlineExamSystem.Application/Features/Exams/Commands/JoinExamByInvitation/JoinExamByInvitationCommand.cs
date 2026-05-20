using MediatR;
using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Application.Features.Exams.Commands.JoinExamByInvitation
{
    public record JoinExamByInvitationCommand(JoinExamDto JoinInfo) : IRequest<JoinExamResultDto>;

    public class JoinExamResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? ExamId { get; set; }
        public int? AttemptId { get; set; }
        public string? RedirectUrl { get; set; }
    }
}