using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExamSystem.Domains.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public string Action { get; set; } = ""; // Create / Update / Delete
        public string EntityName { get; set; } = "";    
        public string EntityId { get; set; } = "";

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
