using MediatR;
using OnlineExamSystem.Application.DTOs.Analytics;

namespace OnlineExamSystem.Application.Features.Analytics.Queries.GetStudentsPerformance
{
    public record GetStudentsPerformanceQuery(int ExamId) : IRequest<List<StudentPerformanceDto>>;
}