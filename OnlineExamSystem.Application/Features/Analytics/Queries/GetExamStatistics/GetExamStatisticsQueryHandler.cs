using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Analytics;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Analytics.Queries.GetExamStatistics
{
    public class GetExamStatisticsQueryHandler
        : IRequestHandler<GetExamStatisticsQuery, ExamStatisticsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetExamStatisticsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamStatisticsDto> Handle(GetExamStatisticsQuery request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.ExamId);
            if (exam == null) return null!;

            var submissions = await _unitOfWork.Repository<ExamSubmission>()
                .GetQueryable()
                .Where(s => s.ExamId == request.ExamId)
                .ToListAsync(cancellationToken);

            var totalSubmissions = submissions.Count;
            if (totalSubmissions == 0)
            {
                return new ExamStatisticsDto
                {
                    ExamId = exam.ExamId,
                    ExamTitle = exam.Title,
                    TotalSubmissions = 0,
                    TotalStudents = 0,
                    AverageScore = 0,
                    PassRate = 0,
                    ScoreDistribution = new List<ScoreDistributionDto>()
                };
            }

            var averageScore = submissions.Average(s => s.Score);
            var passCount = submissions.Count(s => s.IsPassed);
            var passRate = (double)passCount / totalSubmissions * 100;

            // Score Distribution (0-20, 20-40, ..., 80-100)
            var distribution = new List<ScoreDistributionDto>();
            var ranges = new[] { 0, 20, 40, 60, 80, 100 };
            for (int i = 0; i < ranges.Length - 1; i++)
            {
                var low = ranges[i];
                var high = ranges[i + 1];
                var count = submissions.Count(s => s.Score >= low && s.Score <= high);
                var percentage = totalSubmissions > 0 ? (double)count / totalSubmissions * 100 : 0;
                distribution.Add(new ScoreDistributionDto
                {
                    Range = $"{low}-{high}%",
                    Count = count,
                    Percentage = percentage
                });
            }

            return new ExamStatisticsDto
            {
                ExamId = exam.ExamId,
                ExamTitle = exam.Title,
                TotalSubmissions = totalSubmissions,
                TotalStudents = totalSubmissions, // Assuming for now
                AverageScore = averageScore,
                PassRate = passRate,
                ScoreDistribution = distribution
            };
        }
    }
}