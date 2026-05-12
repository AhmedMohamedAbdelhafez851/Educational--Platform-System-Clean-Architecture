using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Analytics;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Analytics.Queries.GetQuestionPerformance
{
    public class GetQuestionPerformanceQueryHandler
        : IRequestHandler<GetQuestionPerformanceQuery, List<QuestionPerformanceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetQuestionPerformanceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<QuestionPerformanceDto>> Handle(GetQuestionPerformanceQuery request, CancellationToken cancellationToken)
        {
            // Get all questions for the exam
            var questions = await _unitOfWork.Repository<Question>()
                .GetQueryable()
                .Where(q => q.ExamId == request.ExamId)
                .Include(q => q.Choices)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Get all answers for these questions
            var questionIds = questions.Select(q => q.QuestionId).ToList();
            var answers = await _unitOfWork.Repository<UserAnswer>()
                .GetQueryable()
                .Where(a => questionIds.Contains(a.QuestionId))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var performance = new List<QuestionPerformanceDto>();

            foreach (var question in questions)
            {
                var questionAnswers = answers.Where(a => a.QuestionId == question.QuestionId).ToList();
                var totalAttempts = questionAnswers.Count;
                var correctCount = questionAnswers.Count(a => a.SelectedChoiceId == question.CorrectChoiceId);

                performance.Add(new QuestionPerformanceDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.Title,
                    TotalAttempts = totalAttempts,
                    CorrectCount = correctCount,
                    CorrectPercentage = totalAttempts > 0 ? (double)correctCount / totalAttempts * 100 : 0
                });
            }

            return performance.OrderBy(p => p.CorrectPercentage).ToList(); // Hardest first
        }
    }
}