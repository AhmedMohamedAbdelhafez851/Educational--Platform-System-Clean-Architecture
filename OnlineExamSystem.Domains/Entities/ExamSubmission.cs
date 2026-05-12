namespace OnlineExamSystem.Domains.Entities
{
    public class ExamSubmission
    {
        public int SubmissionId { get; set; }
        public string UserId { get; set; } = "";
        public int ExamId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double Score { get; set; }
        public bool IsPassed { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Exam Exam { get; set; } = null!;
        public List<UserAnswer> Answers { get; set; } = new();
    }
}