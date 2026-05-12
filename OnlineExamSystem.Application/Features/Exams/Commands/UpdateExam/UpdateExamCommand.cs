using MediatR;
using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Application.Features.Exams.Commands.UpdateExam
{
    public record UpdateExamCommand(int Id, CreateExamDto Dto) : IRequest<Unit>;
}