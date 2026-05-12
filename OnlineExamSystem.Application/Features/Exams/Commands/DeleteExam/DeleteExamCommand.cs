using MediatR;

namespace OnlineExamSystem.Application.Features.Exams.Commands.DeleteExam
{
    public record DeleteExamCommand(int Id) : IRequest<Unit>;
}