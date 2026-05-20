using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExamSystem.Domains.Entities
{
    namespace OnlineExamSystem.Domains.Entities
    {
        public class ExamInvitation
        {
            public int Id { get; set; }
            public int ExamId { get; set; }
            public string Token { get; set; } = ""; // Unique GUID
            public string InvitationCode { get; set; } = ""; // Short code for easy sharing
            public DateTime CreatedAt { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public int MaxAttempts { get; set; } = 1;
            public bool IsActive { get; set; } = true;
            public string? CreatedByUserId { get; set; }

            // Navigation
            public Exam Exam { get; set; } = null!;
            public List<ExamInvitationAttempt> Attempts { get; set; } = new();
        }
    }
}
