using MediatR;
using OnlineExamSystem.Application.DTOs.Analytics;

namespace OnlineExamSystem.Application.Features.Analytics.Queries.GetQuestionPerformance
{
    public record GetQuestionPerformanceQuery(int ExamId) : IRequest<List<QuestionPerformanceDto>>;
}