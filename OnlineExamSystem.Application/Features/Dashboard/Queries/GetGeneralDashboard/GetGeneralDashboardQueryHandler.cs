using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Dashboard;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Dashboard.Queries.GetGeneralDashboard
{
    public class GetGeneralDashboardQueryHandler : IRequestHandler<GetGeneralDashboardQuery, GeneralDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetGeneralDashboardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralDashboardDto> Handle(GetGeneralDashboardQuery request, CancellationToken cancellationToken)
        {
            // Get all exams
            var exams = await _unitOfWork.Repository<Exam>()
                .GetQueryable()
                .Include(e => e.Submissions)
                .ToListAsync(cancellationToken);

            // Get all submissions (including anonymous ones)
            var allSubmissions = await _unitOfWork.Repository<ExamSubmission>()
                .GetQueryable()
                .Include(s => s.User)
                .ToListAsync(cancellationToken);

            // Get all invitation attempts to track unique students
            var allInvitationAttempts = await _unitOfWork.Repository<ExamInvitationAttempt>()
                .GetQueryable()
                .ToListAsync(cancellationToken);

            // Calculate unique students from both submissions and invitations
            var studentsFromSubmissions = allSubmissions
                .Where(s => !string.IsNullOrEmpty(s.StudentName))
                .Select(s => s.StudentName)
                .Distinct()
                .ToList();

            var studentsFromInvitations = allInvitationAttempts
                .Where(a => !string.IsNullOrEmpty(a.StudentName))
                .Select(a => a.StudentName)
                .Distinct()
                .ToList();

            var allUniqueStudents = studentsFromSubmissions
                .Union(studentsFromInvitations)
                .Distinct()
                .Count();

            var totalExams = exams.Count;
            var examsThisWeek = exams.Count(e => e.CreatedDate >= DateTime.UtcNow.AddDays(-7));

            var allScores = allSubmissions.Select(s => s.Score).ToList();
            var averageScore = allScores.Any() ? allScores.Average() : 0;

            // Students needing attention (failed exams)
            var studentsNeedingAttention = allSubmissions
                .Where(s => !s.IsPassed)
                .Select(s => s.StudentName ?? s.User?.UserName)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .Count();

            // Calculate attendance rate based on actual submissions vs expected
            var totalPossibleSubmissions = totalExams * Math.Max(allUniqueStudents, 1);
            var actualSubmissions = allSubmissions.Count;
            var attendanceRate = totalPossibleSubmissions > 0 ? (double)actualSubmissions / totalPossibleSubmissions * 100 : 0;

            // Recent Exams (last 5)
            var recentExams = exams
                .OrderByDescending(e => e.CreatedDate)
                .Take(5)
                .Select(e => new RecentExamDto
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    Subject = "General",
                    Date = e.CreatedDate,
                    AttendancePercentage = e.Submissions.Any() && allUniqueStudents > 0 ? (double)e.Submissions.Count / allUniqueStudents * 100 : 0,
                    AverageScore = e.Submissions.Any() ? e.Submissions.Average(s => s.Score) : 0
                }).ToList();

            // Students Needing Attention (failed multiple exams)
            var studentsNeedingAttentionList = allSubmissions
                .Where(s => !s.IsPassed)
                .GroupBy(s => s.StudentName ?? s.User?.UserName)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .Select(g => new
                {
                    StudentName = g.Key,
                    FailedCount = g.Count(),
                    LatestScore = g.OrderByDescending(s => s.SubmissionDate).First().Score
                })
                .OrderByDescending(x => x.FailedCount)
                .Take(5)
                .Select(x => new AttentionStudentDto
                {
                    UserId = "",
                    StudentName = x.StudentName ?? "Student",
                    Issue = $"Failed {x.FailedCount} exam(s)",
                    LatestScore = x.LatestScore,
                    Status = "Critical"
                }).ToList();

            // Recent Activities
            var recentActivities = new List<ActivityDto>();

            var recentSubmissions = allSubmissions
                .OrderByDescending(s => s.SubmissionDate)
                .Take(10)
                .ToList();

            foreach (var sub in recentSubmissions)
            {
                var exam = exams.FirstOrDefault(e => e.ExamId == sub.ExamId);
                var studentName = sub.StudentName ?? sub.User?.UserName ?? "Anonymous";
                recentActivities.Add(new ActivityDto
                {
                    Message = $"{studentName} completed exam: {exam?.Title ?? "Unknown"}",
                    Timestamp = sub.SubmissionDate,
                    Icon = sub.IsPassed ? "check-circle-fill" : "x-circle-fill",
                    Color = sub.IsPassed ? "success" : "danger",
                    Type = sub.IsPassed ? "exam_completed" : "exam_failed"
                });
            }

            var recentCreatedExams = exams
                .OrderByDescending(e => e.CreatedDate)
                .Take(5)
                .Select(e => new ActivityDto
                {
                    Message = $"New exam created: {e.Title}",
                    Timestamp = e.CreatedDate,
                    Icon = "file-text",
                    Color = "primary",
                    Type = "exam_created"
                });

            recentActivities.AddRange(recentCreatedExams);
            recentActivities = recentActivities.OrderByDescending(a => a.Timestamp).Take(10).ToList();

            // Upcoming Exams
            var upcomingExams = exams
                .Take(5)
                .Select(e => new UpcomingExamDto
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    DateTime = e.CreatedDate.AddDays(7),
                    StudentCount = allUniqueStudents,
                    Status = "Scheduled"
                }).ToList();

            // Smart Insights
            var smartInsights = GenerateInsights(averageScore, studentsNeedingAttention, attendanceRate, exams);

            return new GeneralDashboardDto
            {
                TotalStudents = allUniqueStudents,
                TotalExams = totalExams,
                ExamsThisWeek = examsThisWeek,
                AverageStudentScore = averageScore,
                StudentsNeedingAttention = studentsNeedingAttention,
                AttendanceRate = attendanceRate,
                RecentExams = recentExams,
                StudentsNeedingAttentionList = studentsNeedingAttentionList,
                RecentActivities = recentActivities,
                UpcomingExams = upcomingExams,
                SmartInsights = smartInsights
            };
        }

        private List<InsightDto> GenerateInsights(double averageScore, int strugglingStudents, double attendanceRate, List<Exam> exams)
        {
            var insights = new List<InsightDto>();

            if (averageScore >= 75)
            {
                insights.Add(new InsightDto
                {
                    Message = $"Excellent performance! Average score is {averageScore:F1}%",
                    Type = "success",
                    ActionText = "View details",
                    ActionUrl = "/Exam"
                });
            }
            else if (averageScore >= 60)
            {
                insights.Add(new InsightDto
                {
                    Message = $"Good performance. Average score is {averageScore:F1}%",
                    Type = "info",
                    ActionText = "View details",
                    ActionUrl = "/Exam"
                });
            }
            else if (averageScore > 0)
            {
                insights.Add(new InsightDto
                {
                    Message = $"Student performance needs improvement. Average score is {averageScore:F1}%",
                    Type = "warning",
                    ActionText = "View struggling students",
                    ActionUrl = "/Users"
                });
            }

            if (strugglingStudents > 0)
            {
                insights.Add(new InsightDto
                {
                    Message = $"{strugglingStudents} students need immediate attention",
                    Type = "warning",
                    ActionText = "Review students",
                    ActionUrl = "/Users"
                });
            }

            if (attendanceRate < 70 && attendanceRate > 0)
            {
                insights.Add(new InsightDto
                {
                    Message = $"Attendance rate is low ({attendanceRate:F1}%). Consider engaging students",
                    Type = "warning",
                    ActionText = "View attendance"
                });
            }

            if (exams.Count > 0)
            {
                var lastExam = exams.OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                if (lastExam != null)
                {
                    insights.Add(new InsightDto
                    {
                        Message = $"Recent exam: {lastExam.Title} was created",
                        Type = "info",
                        ActionText = "View analytics",
                        ActionUrl = $"/Analytics/ExamReport?examId={lastExam.ExamId}"
                    });
                }
            }

            return insights;
        }
    }
}