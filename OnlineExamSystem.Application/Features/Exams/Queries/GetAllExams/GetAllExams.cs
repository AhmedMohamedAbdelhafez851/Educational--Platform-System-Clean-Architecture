using MediatR;
using OnlineExamSystem.Application.Caching;
using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Application.Features.Exams.Queries.GetAllExams
{
    public class GetAllExamsQuery :
        IRequest<List<ExamListDto>>,
        ICacheableQuery
    {
        public string CacheKey =>
            "AllExams";

        public int SlidingExpirationInMinutes =>
            5;
    }
}