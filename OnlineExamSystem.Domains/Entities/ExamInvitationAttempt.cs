using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Domains.Entities
{
    public class ExamInvitationAttempt
    {
        public int Id { get; set; }
        public int InvitationId { get; set; }
        public string StudentName { get; set; } = "";
        public string? StudentEmail { get; set; }
        public string? StudentId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? SubmissionId { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation
        public ExamInvitation Invitation { get; set; } = null!;
        public ExamSubmission? Submission { get; set; }
    }
}