namespace OnlineExamSystem.Domains.Entities
{
    public class UserAnswer
    {
        public int UserAnswerId { get; set; }
        public int SubmissionId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedChoiceId { get; set; }

        public ExamSubmission Submission { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public Choice SelectedChoice { get; set; } = null!;
    }
}