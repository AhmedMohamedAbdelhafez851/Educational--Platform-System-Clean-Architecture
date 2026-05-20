using System.ComponentModel.DataAnnotations;

namespace OnlineExamSystem.Domains.Entities
{
    public class ExamSubmission
    {
        [Key]
        public int SubmissionId { get; set; }

        public string? UserId { get; set; }
        public int ExamId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double Score { get; set; }
        public bool IsPassed { get; set; }

        // Anonymous student info
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string? StudentId { get; set; }

        // Navigation properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Exam Exam { get; set; } = null!;
        public virtual ICollection<UserAnswer> Answers { get; set; } = new List<UserAnswer>();
    }
}