using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Services
{
    public class ExamSubmissionService : IExamSubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamSubmissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ExamSubmission>> GetUserSubmissionsAsync(string userId)
        {
            return await _unitOfWork.Repository<ExamSubmission>()
                .GetListAsync(es => es.UserId == userId, orderBy: q => q.OrderByDescending(x => x.SubmissionDate));
        }

        public async Task<ExamSubmission?> GetSubmissionDetailsAsync(int submissionId, string userId)
        {
            var submission = await (await _unitOfWork.Repository<ExamSubmission>()
                    .GetAllWithNestedIncludesAsync(q => q
                        .Include(es => es.Exam)
                        .Include(es => es.Answers)
                            .ThenInclude(a => a.Question)
                                .ThenInclude(q => q.Choices)
                        .Include(es => es.Answers)
                            .ThenInclude(a => a.SelectedChoice)))
                .FirstOrDefaultAsync(es => es.SubmissionId == submissionId && es.UserId == userId);

            return submission;
        }

        public async Task<ExamSubmission> SubmitExamAsync(string userId, int examId, Dictionary<int, int> answers)
        {
            var examQuery = await _unitOfWork.Repository<Exam>()
                .GetAllWithNestedIncludesAsync(q => q
                    .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices));

            var exam = await examQuery.FirstOrDefaultAsync(e => e.ExamId == examId);
            if (exam == null) throw new Exception("Exam not found.");

            var submission = new ExamSubmission
            {
                UserId = userId,
                ExamId = examId,
                SubmissionDate = DateTime.UtcNow,
                TotalQuestions = exam.Questions.Count,
                Answers = new List<UserAnswer>()
            };

            int correctAnswers = 0;

            foreach (var answer in answers)
            {
                var question = exam.Questions.FirstOrDefault(q => q.QuestionId == answer.Key);
                if (question != null)
                {
                    submission.Answers.Add(new UserAnswer
                    {
                        QuestionId = question.QuestionId,
                        SelectedChoiceId = answer.Value
                    });

                    if (question.CorrectChoiceId.HasValue && question.CorrectChoiceId.Value == answer.Value)
                        correctAnswers++;
                }
            }

            submission.CorrectAnswers = correctAnswers;
            submission.Score = submission.TotalQuestions > 0 ? (double)correctAnswers / submission.TotalQuestions * 100 : 0;
            submission.IsPassed = submission.Score >= 50;

            await _unitOfWork.Repository<ExamSubmission>().AddAsync(submission);
            await _unitOfWork.SaveChangesAsync();

            return submission;
        }
    }
}