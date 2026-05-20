using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Analytics;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Analytics.Queries.GetExamAnalytics
{
    public class GetExamAnalyticsQueryHandler : IRequestHandler<GetExamAnalyticsQuery, ExamAnalyticsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetExamAnalyticsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamAnalyticsDto> Handle(GetExamAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>()
                .GetByIdAsync(request.ExamId);

            if (exam == null) throw new Exception("Exam not found");

            // Get all submissions for this exam
            var submissions = await _unitOfWork.Repository<ExamSubmission>()
                .GetQueryable()
                .Where(s => s.ExamId == request.ExamId)
                .Include(s => s.User)
                .OrderByDescending(s => s.Score)
                .ToListAsync(cancellationToken);

            // Get all invitation attempts for this exam
            var allInvitationAttempts = await _unitOfWork.Repository<ExamInvitationAttempt>()
                .GetQueryable()
                .Where(a => a.Invitation.ExamId == request.ExamId)
                .ToListAsync(cancellationToken);

            // Calculate unique students from submissions
            var studentsFromSubmissions = submissions
                .Where(s => !string.IsNullOrEmpty(s.StudentName))
                .Select(s => s.StudentName)
                .Distinct()
                .ToList();

            // Calculate unique students from invitations
            var studentsFromInvitations = allInvitationAttempts
                .Where(a => !string.IsNullOrEmpty(a.StudentName))
                .Select(a => a.StudentName)
                .Distinct()
                .ToList();

            // Combine all unique students
            var allUniqueStudents = studentsFromSubmissions
                .Union(studentsFromInvitations)
                .Distinct()
                .ToList();

            var totalDistinctStudents = allUniqueStudents.Count;
            var studentsAttended = submissions.Count;
            var studentsNotAttended = totalDistinctStudents - studentsAttended;

            // Calculate score statistics
            var scores = submissions.Select(s => s.Score).ToList();
            var averageScore = scores.Any() ? scores.Average() : 0;
            var highestScore = scores.Any() ? scores.Max() : 0;
            var lowestScore = scores.Any() ? scores.Min() : 0;
            var passCount = submissions.Count(s => s.IsPassed);
            var passRate = studentsAttended > 0 ? (double)passCount / studentsAttended * 100 : 0;

            // Score distribution
            var distribution = new List<ScoreDistributionDto>();
            var ranges = new[] { 0, 20, 40, 60, 80, 100 };
            for (int i = 0; i < ranges.Length - 1; i++)
            {
                var low = ranges[i];
                var high = ranges[i + 1];
                var count = submissions.Count(s => s.Score >= low && s.Score <= high);
                var percentage = studentsAttended > 0 ? (double)count / studentsAttended * 100 : 0;
                distribution.Add(new ScoreDistributionDto
                {
                    Range = $"{low}-{high}%",
                    Count = count,
                    Percentage = percentage
                });
            }

            // Get question performance (most difficult questions)
            var questions = await _unitOfWork.Repository<Question>()
                .GetQueryable()
                .Where(q => q.ExamId == request.ExamId)
                .Include(q => q.Choices)
                .ToListAsync(cancellationToken);

            var allAnswers = await _unitOfWork.Repository<UserAnswer>()
                .GetQueryable()
                .Where(a => submissions.Select(s => s.SubmissionId).Contains(a.SubmissionId))
                .ToListAsync(cancellationToken);

            var difficultQuestions = new List<QuestionPerformanceDto>();
            foreach (var question in questions)
            {
                var questionAnswers = allAnswers.Where(a => a.QuestionId == question.QuestionId).ToList();
                var totalAttempts = questionAnswers.Count;
                var correctCount = questionAnswers.Count(a => a.SelectedChoiceId == question.CorrectChoiceId);
                var correctPercentage = totalAttempts > 0 ? (double)correctCount / totalAttempts * 100 : 0;

                string difficulty = correctPercentage >= 70 ? "Easy" : correctPercentage >= 40 ? "Medium" : "Hard";

                difficultQuestions.Add(new QuestionPerformanceDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.Title.Length > 100 ? question.Title.Substring(0, 100) + "..." : question.Title,
                    TotalAttempts = totalAttempts,
                    CorrectCount = correctCount,
                    CorrectPercentage = correctPercentage,
                    Difficulty = difficulty
                });
            }

            // Sort by difficulty (hardest first)
            difficultQuestions = difficultQuestions.OrderBy(q => q.CorrectPercentage).ToList();

            // Get top students (Top 5)
            var topStudents = submissions
                .OrderByDescending(s => s.Score)
                .Take(5)
                .Select(s => new StudentPerformanceDto
                {
                    UserId = s.UserId ?? "anonymous",
                    StudentName = s.StudentName ?? s.User?.UserName ?? "Anonymous",
                    Email = s.StudentEmail ?? s.User?.Email ?? "",
                    Score = s.Score,
                    IsPassed = s.IsPassed,
                    SubmissionDate = s.SubmissionDate,
                    CorrectAnswers = s.CorrectAnswers,
                    TotalQuestions = s.TotalQuestions
                }).ToList();

            // Get struggling students (Bottom 5, who took the exam and failed or scored low)
            var strugglingStudents = submissions
                .Where(s => s.Score < 50)
                .OrderBy(s => s.Score)
                .Take(5)
                .Select(s => new StudentPerformanceDto
                {
                    UserId = s.UserId ?? "anonymous",
                    StudentName = s.StudentName ?? s.User?.UserName ?? "Anonymous",
                    Email = s.StudentEmail ?? s.User?.Email ?? "",
                    Score = s.Score,
                    IsPassed = s.IsPassed,
                    SubmissionDate = s.SubmissionDate,
                    CorrectAnswers = s.CorrectAnswers,
                    TotalQuestions = s.TotalQuestions
                }).ToList();

            // Build attendance list - FIXED to show ALL invited students correctly
            var attendanceList = new List<StudentAttendanceDto>();

            // Add students who took the exam (from submissions)
            foreach (var sub in submissions)
            {
                var studentName = sub.StudentName ?? sub.User?.UserName ?? "Anonymous Student";

                attendanceList.Add(new StudentAttendanceDto
                {
                    StudentName = studentName,
                    Email = sub.StudentEmail ?? sub.User?.Email ?? "",
                    StudentId = sub.StudentId,
                    Attended = true,
                    SubmissionDate = sub.SubmissionDate,
                    Score = sub.Score,
                    IsPassed = sub.IsPassed
                });
            }

            // Add students who were invited but didn't attend
            foreach (var attempt in allInvitationAttempts)
            {
                var hasSubmitted = submissions.Any(s =>
                    s.StudentName == attempt.StudentName ||
                    s.StudentEmail == attempt.StudentEmail ||
                    s.StudentId == attempt.StudentId);

                if (!hasSubmitted && !attendanceList.Any(a => a.StudentName == attempt.StudentName))
                {
                    attendanceList.Add(new StudentAttendanceDto
                    {
                        StudentName = attempt.StudentName,
                        Email = attempt.StudentEmail ?? "",
                        StudentId = attempt.StudentId,
                        Attended = false,
                        SubmissionDate = null,
                        Score = null,
                        IsPassed = null
                    });
                }
            }

            return new ExamAnalyticsDto
            {
                ExamId = exam.ExamId,
                ExamTitle = exam.Title,
                TotalStudents = totalDistinctStudents,
                StudentsAttended = studentsAttended,
                StudentsNotAttended = studentsNotAttended,
                AttendanceRate = totalDistinctStudents > 0 ? (double)studentsAttended / totalDistinctStudents * 100 : 0,
                AverageScore = averageScore,
                HighestScore = highestScore,
                LowestScore = lowestScore,
                PassRate = passRate,
                ScoreDistribution = distribution,
                DifficultQuestions = difficultQuestions,
                TopStudents = topStudents,
                StrugglingStudents = strugglingStudents,
                AttendanceList = attendanceList
            };
        }
    }
}