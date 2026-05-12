using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Queries.GetAllExams
{
    public class GetAllExamsQueryHandler : IRequestHandler<GetAllExamsQuery, List<ExamListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public GetAllExamsQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<List<ExamListDto>> Handle(GetAllExamsQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "AllExams";

            var cached = await _cacheService.GetAsync<List<ExamListDto>>(cacheKey);
            if (cached != null) return cached;

            var exams = await _unitOfWork.Repository<Exam>()
                .GetQueryable()
                .AsNoTracking()
                .Select(e => new ExamListDto
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    DurationInMinutes = e.DurationInMinutes,
                    TotalDegree = e.TotalDegree,
                    QuestionsCount = e.Questions.Count()
                })
                .ToListAsync(cancellationToken);

            await _cacheService.SetAsync(cacheKey, exams, TimeSpan.FromMinutes(5));

            return exams;
        }
    }
}