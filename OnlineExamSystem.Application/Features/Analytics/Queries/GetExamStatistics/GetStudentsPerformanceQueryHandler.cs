using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Analytics;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Analytics.Queries.GetStudentsPerformance
{
    public class GetStudentsPerformanceQueryHandler
        : IRequestHandler<GetStudentsPerformanceQuery, List<StudentPerformanceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudentsPerformanceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentPerformanceDto>> Handle(GetStudentsPerformanceQuery request, CancellationToken cancellationToken)
        {
            var submissions = await _unitOfWork.Repository<ExamSubmission>()
                .GetQueryable()
                .Where(s => s.ExamId == request.ExamId)
                .Include(s => s.User)
                .AsNoTracking()
                .Select(s => new StudentPerformanceDto
                {
                    UserId = s.UserId,
                    UserName = s.User.UserName ?? s.User.Email ?? "",
                    Email = s.User.Email ?? "",
                    Score = s.Score,
                    IsPassed = s.IsPassed,
                    SubmissionDate = s.SubmissionDate
                })
                .ToListAsync(cancellationToken);

            return submissions.OrderByDescending(s => s.Score).ToList();
        }
    }
}