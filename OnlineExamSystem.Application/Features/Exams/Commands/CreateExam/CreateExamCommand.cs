
using MediatR;
using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Application.Features.Exams.Commands.CreateExam
{
    public record CreateExamCommand(CreateExamDto Dto) : IRequest<int>;
}
