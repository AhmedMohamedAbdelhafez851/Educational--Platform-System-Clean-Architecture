using Microsoft.AspNetCore.Identity;

namespace OnlineExamSystem.Domains.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}