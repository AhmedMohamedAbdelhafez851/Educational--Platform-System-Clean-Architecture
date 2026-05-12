using MediatR;
using OnlineExamSystem.Application.DTOs.Analytics;

namespace OnlineExamSystem.Application.Features.Analytics.Queries.GetExamStatistics
{
    public record GetExamStatisticsQuery(int ExamId) : IRequest<ExamStatisticsDto>;
}