namespace OnlineExamSystem.Application.DTOs.Analytics
{
    public class ExamStatisticsDto
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = "";
        public int TotalSubmissions { get; set; } // Who attended
        public int TotalStudents { get; set; } // Total assigned (to be implemented later, set = TotalSubmissions for now)
        public double AverageScore { get; set; }
        public double PassRate { get; set; }

        // For bar chart
        public List<ScoreDistributionDto> ScoreDistribution { get; set; } = new();
    }

    public class ScoreDistributionDto
    {
        public string Range { get; set; } = ""; // e.g., "0-20%"
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}