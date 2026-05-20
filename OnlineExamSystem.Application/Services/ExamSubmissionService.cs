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

        public async Task<ExamSubmission?> GetSubmissionDetailsAsync(int submissionId, string? userId = null)
        {
            var query = await _unitOfWork.Repository<ExamSubmission>()
                .GetAllWithNestedIncludesAsync(q => q
                    .Include(es => es.Exam)
                    .Include(es => es.Answers)
                        .ThenInclude(a => a.Question)
                            .ThenInclude(q => q.Choices)
                    .Include(es => es.Answers)
                        .ThenInclude(a => a.SelectedChoice));

            if (!string.IsNullOrEmpty(userId))
            {
                return await query.FirstOrDefaultAsync(es => es.SubmissionId == submissionId && es.UserId == userId);
            }

            return await query.FirstOrDefaultAsync(es => es.SubmissionId == submissionId);
        }

        public async Task<ExamSubmission> SubmitExamAsync(string userId, int examId, Dictionary<int, int> answers, string studentName = "", string studentEmail = "", string studentId = "")
        {
            var examQuery = await _unitOfWork.Repository<Exam>()
                .GetAllWithNestedIncludesAsync(q => q
                    .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices));

            var exam = await examQuery.FirstOrDefaultAsync(e => e.ExamId == examId);
            if (exam == null) throw new Exception("Exam not found.");

            // Check if user exists in database
            var userExists = await _unitOfWork.Repository<ApplicationUser>()
                .AnyAsync(u => u.Id == userId);

            // IMPORTANT: Use the provided student name, don't override with "Anonymous"
            var finalStudentName = !string.IsNullOrEmpty(studentName) ? studentName : (userExists ? null : "Anonymous Student");
            var finalStudentEmail = !string.IsNullOrEmpty(studentEmail) ? studentEmail : null;
            var finalStudentId = !string.IsNullOrEmpty(studentId) ? studentId : null;

            var submission = new ExamSubmission
            {
                SubmissionDate = DateTime.UtcNow,
                ExamId = examId,
                TotalQuestions = exam.Questions.Count,
                CorrectAnswers = 0,
                Score = 0,
                IsPassed = false,
                Answers = new List<UserAnswer>(),
                StudentName = finalStudentName,
                StudentEmail = finalStudentEmail,
                StudentId = finalStudentId
            };

            // Only set UserId if user exists in database
            if (userExists)
            {
                submission.UserId = userId;
            }

            int correctAnswers = 0;

            foreach (var answer in answers)
            {
                var question = exam.Questions.FirstOrDefault(q => q.QuestionId == answer.Key);
                if (question != null)
                {
                    var userAnswer = new UserAnswer
                    {
                        QuestionId = question.QuestionId,
                        SelectedChoiceId = answer.Value
                    };
                    submission.Answers.Add(userAnswer);

                    if (question.CorrectChoiceId.HasValue && question.CorrectChoiceId.Value == answer.Value)
                    {
                        correctAnswers++;
                    }
                }
            }

            submission.CorrectAnswers = correctAnswers;
            submission.Score = submission.TotalQuestions > 0 ? (double)correctAnswers / submission.TotalQuestions * 100 : 0;
            submission.IsPassed = submission.Score >= 50;

            await _unitOfWork.Repository<ExamSubmission>().AddAsync(submission);
            await _unitOfWork.SaveChangesAsync();

            // Update each answer with the correct SubmissionId
            foreach (var answer in submission.Answers)
            {
                answer.SubmissionId = submission.SubmissionId;
            }

            await _unitOfWork.SaveChangesAsync();

            // Update the invitation attempt with the submission ID
            var attempt = await _unitOfWork.Repository<ExamInvitationAttempt>()
                .GetQueryable()
                .FirstOrDefaultAsync(a => a.StudentName == studentName && a.Invitation.ExamId == examId && !a.IsCompleted);

            if (attempt != null)
            {
                attempt.SubmissionId = submission.SubmissionId;
                attempt.CompletedAt = DateTime.UtcNow;
                attempt.IsCompleted = true;
                await _unitOfWork.SaveChangesAsync();
            }

            return submission;
        }
    }
}