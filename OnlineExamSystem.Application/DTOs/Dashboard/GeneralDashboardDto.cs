namespace OnlineExamSystem.Application.DTOs.Dashboard
{
    public class GeneralDashboardDto
    {
        // KPI Cards
        public int TotalStudents { get; set; }
        public int TotalExams { get; set; }
        public int ExamsThisWeek { get; set; }
        public double AverageStudentScore { get; set; }
        public int StudentsNeedingAttention { get; set; }
        public double AttendanceRate { get; set; }

        // Recent Exams
        public List<RecentExamDto> RecentExams { get; set; } = new();

        // Students Needing Attention
        public List<AttentionStudentDto> StudentsNeedingAttentionList { get; set; } = new();

        // Recent Activity
        public List<ActivityDto> RecentActivities { get; set; } = new();

        // Upcoming Exams
        public List<UpcomingExamDto> UpcomingExams { get; set; } = new();

        // Smart Insights
        public List<InsightDto> SmartInsights { get; set; } = new();
    }

    public class RecentExamDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = "";
        public string Subject { get; set; } = "";
        public DateTime Date { get; set; }
        public double AttendancePercentage { get; set; }
        public double AverageScore { get; set; }
    }

    public class AttentionStudentDto
    {
        public string UserId { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string Issue { get; set; } = "";
        public double LatestScore { get; set; }
        public string Status { get; set; } = "";
    }

    public class ActivityDto
    {
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = "";
        public string Color { get; set; } = "";
        public string Type { get; set; } = ""; // exam_started, exam_completed, exam_failed, exam_created
    }

    public class UpcomingExamDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = "";
        public DateTime DateTime { get; set; }
        public int StudentCount { get; set; }
        public string Status { get; set; } = "";
    }

    public class InsightDto
    {
        public string Message { get; set; } = "";
        public string Type { get; set; } = ""; // success, warning, info
        public string ActionText { get; set; } = "";
        public string ActionUrl { get; set; } = "";
    }
}