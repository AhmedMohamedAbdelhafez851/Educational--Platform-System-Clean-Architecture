using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IAuditService
    {
        Task LogAsync(
            string userId,
            string action,
            string entityName,
            string entityId,
            object? oldValues = null,
            object? newValues = null);
    }
}
