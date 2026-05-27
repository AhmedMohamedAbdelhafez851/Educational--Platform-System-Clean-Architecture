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
                    StudentName = x.StudentName ?? "طالب",
                    Issue = $"رسب في {x.FailedCount} امتحان(ات)",
                    LatestScore = x.LatestScore,
                    Status = "حرج"
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
                var studentName = sub.StudentName ?? sub.User?.UserName ?? "طالب مجهول";
                recentActivities.Add(new ActivityDto
                {
                    Message = $"{studentName} أكمل الامتحان: {exam?.Title ?? "غير معروف"}",
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
                    Message = $"تم إنشاء امتحان جديد: {e.Title}",
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
                    Status = "مجدول"
                }).ToList();

            // Smart Insights - Arabic Version
            var smartInsights = GenerateArabicInsights(averageScore, studentsNeedingAttention, attendanceRate, exams);

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

        private List<InsightDto> GenerateArabicInsights(double averageScore, int strugglingStudents, double attendanceRate, List<Exam> exams)
        {
            var insights = new List<InsightDto>();

            // Insight 1: Average Score Performance
            if (averageScore >= 75)
            {
                insights.Add(new InsightDto
                {
                    Message = $"⭐ أداء ممتاز! متوسط الدرجات {averageScore:F1}%",
                    Type = "success",
                    ActionText = "عرض التفاصيل",
                    ActionUrl = "/Exam"
                });
            }
            else if (averageScore >= 60)
            {
                insights.Add(new InsightDto
                {
                    Message = $"📊 أداء جيد. متوسط الدرجات {averageScore:F1}%",
                    Type = "info",
                    ActionText = "عرض التفاصيل",
                    ActionUrl = "/Exam"
                });
            }
            else if (averageScore > 0)
            {
                insights.Add(new InsightDto
                {
                    Message = $"⚠️ أداء الطلاب بحاجة إلى تحسين. متوسط الدرجات {averageScore:F1}%",
                    Type = "warning",
                    ActionText = "عرض الطلاب",
                    ActionUrl = "/Users"
                });
            }

            // Insight 2: Struggling Students
            if (strugglingStudents > 0)
            {
                insights.Add(new InsightDto
                {
                    Message = strugglingStudents == 1
                        ? $"👨‍🎓 {strugglingStudents} طالب بحاجة إلى متابعة فورية"
                        : $"👥 {strugglingStudents} طلاب بحاجة إلى متابعة فورية",
                    Type = "warning",
                    ActionText = "مراجعة الطلاب",
                    ActionUrl = "/Users"
                });
            }

            // Insight 3: Attendance Rate
            if (attendanceRate < 70 && attendanceRate > 0)
            {
                string attendanceMessage = attendanceRate < 30
                    ? "⚠️ حرجة"
                    : attendanceRate < 50
                        ? "⚠️ منخفضة جداً"
                        : "⚠️ منخفضة";

                insights.Add(new InsightDto
                {
                    Message = $"📅 نسبة الحضور {attendanceMessage} ({attendanceRate:F1}%). يوصى بتحفيز الطلاب على المشاركة",
                    Type = "warning",
                    ActionText = "عرض الحضور",
                    ActionUrl = "/Users"
                });
            }
            else if (attendanceRate >= 90 && attendanceRate > 0)
            {
                insights.Add(new InsightDto
                {
                    Message = $"🎉 نسبة حضور ممتازة ({attendanceRate:F1}%). استمر بنفس المستوى!",
                    Type = "success",
                    ActionText = "عرض التفاصيل",
                    ActionUrl = "/Users"
                });
            }

            // Insight 4: Recent Exam Created
            if (exams.Count > 0)
            {
                var lastExam = exams.OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                if (lastExam != null)
                {
                    insights.Add(new InsightDto
                    {
                        Message = $"📝 امتحان جديد: \"{lastExam.Title}\" تم إنشاؤه بنجاح",
                        Type = "info",
                        ActionText = "عرض التحليلات",
                        ActionUrl = $"/Analytics/ExamReport?examId={lastExam.ExamId}"
                    });
                }
            }

            // Insight 5: No Exams Yet
            if (exams.Count == 0)
            {
                insights.Add(new InsightDto
                {
                    Message = "✨ مرحباً! ابدأ بإنشاء أول امتحان لك لتفعيل النظام",
                    Type = "info",
                    ActionText = "إنشاء امتحان",
                    ActionUrl = "/Exam/Create"
                });
            }

            // Insight 6: Great Performance - No struggling students
            if (strugglingStudents == 0 && averageScore >= 70 && exams.Count > 0)
            {
                insights.Insert(0, new InsightDto
                {
                    Message = "🏆 أداء رائع! جميع الطلاب يحققون نتائج مميزة. استمر في تقديم محتوى عالي الجودة",
                    Type = "success",
                    ActionText = "عرض الإحصائيات",
                    ActionUrl = "/Exam"
                });
            }

            return insights;
        }
    }
}