using MediatR;
using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Application.Features.Exams.Queries.GetExamById
{
    public record GetExamByIdQuery(int Id) : IRequest<CreateExamDto?>;
}