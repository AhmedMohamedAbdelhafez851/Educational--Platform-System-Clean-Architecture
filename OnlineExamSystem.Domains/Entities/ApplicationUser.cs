
using Microsoft.AspNetCore.Identity;

namespace OnlineExamSystem.Domains.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
    }
}