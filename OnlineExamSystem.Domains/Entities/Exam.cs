namespace OnlineExamSystem.Domains.Entities
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalDegree { get; set; }

        // Navigation properties
        public List<Question> Questions { get; set; } = new();
        public List<ExamSubmission> Submissions { get; set; } = new();
    }
}