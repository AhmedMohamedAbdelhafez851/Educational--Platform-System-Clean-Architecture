namespace OnlineExamSystem.Application.DTOs.Exam
{
    public class ExamInvitationDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = "";
        public string Token { get; set; } = "";
        public string InvitationCode { get; set; } = "";
        public string InvitationLink { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int MaxAttempts { get; set; }
        public bool IsActive { get; set; }
        public int UsedCount { get; set; }
        public List<ExamInvitationAttemptDto> Attempts { get; set; } = new();
    }

    public class ExamInvitationAttemptDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = "";
        public string? StudentEmail { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public double? Score { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class JoinExamDto
    {
        public string InvitationCode { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string? StudentEmail { get; set; }
        public string? StudentId { get; set; }
    }
}