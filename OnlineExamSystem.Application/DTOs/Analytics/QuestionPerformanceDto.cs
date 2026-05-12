namespace OnlineExamSystem.Application.DTOs.Analytics
{
    public class QuestionPerformanceDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public int TotalAttempts { get; set; }
        public int CorrectCount { get; set; }
        public double CorrectPercentage { get; set; } // Hardest = lowest %
    }
}