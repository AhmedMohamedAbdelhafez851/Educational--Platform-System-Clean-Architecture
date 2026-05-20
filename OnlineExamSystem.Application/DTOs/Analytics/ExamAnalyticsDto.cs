namespace OnlineExamSystem.Application.DTOs.Analytics
{
    public class ExamAnalyticsDto
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = "";
        public int TotalStudents { get; set; }
        public int StudentsAttended { get; set; }
        public int StudentsNotAttended { get; set; }
        public double AttendanceRate { get; set; }
        public double AverageScore { get; set; }
        public double HighestScore { get; set; }
        public double LowestScore { get; set; }
        public double PassRate { get; set; }
        public List<ScoreDistributionDto> ScoreDistribution { get; set; } = new();
        public List<QuestionPerformanceDto> DifficultQuestions { get; set; } = new();
        public List<StudentPerformanceDto> TopStudents { get; set; } = new();
        public List<StudentPerformanceDto> StrugglingStudents { get; set; } = new();
        public List<StudentAttendanceDto> AttendanceList { get; set; } = new();
    }

    public class ScoreDistributionDto
    {
        public string Range { get; set; } = "";
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class QuestionPerformanceDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public int TotalAttempts { get; set; }
        public int CorrectCount { get; set; }
        public double CorrectPercentage { get; set; }
        public string Difficulty { get; set; } = "";
    }

    public class StudentPerformanceDto
    {
        public string UserId { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string Email { get; set; } = "";
        public double Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
    }

    public class StudentAttendanceDto
    {
        public string StudentName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? StudentId { get; set; }
        public bool Attended { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public double? Score { get; set; }
        public bool? IsPassed { get; set; }
    }
}