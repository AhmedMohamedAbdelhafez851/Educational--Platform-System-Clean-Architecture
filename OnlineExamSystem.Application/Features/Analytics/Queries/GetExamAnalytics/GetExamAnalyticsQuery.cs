using MediatR;
using OnlineExamSystem.Application.DTOs.Analytics;

namespace OnlineExamSystem.Application.Features.Analytics.Queries.GetExamAnalytics
{
    public record GetExamAnalyticsQuery(int ExamId) : IRequest<ExamAnalyticsDto>;
}