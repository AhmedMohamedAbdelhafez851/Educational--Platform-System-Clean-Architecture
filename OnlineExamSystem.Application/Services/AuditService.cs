using System.Text.Json;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;

public class AuditService : IAuditService
{
    //private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(
        string userId,
        string action,
        string entityName,
        string entityId,
        object? oldValues = null,
        object? newValues = null)
    {
        var audit = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(audit);
        await _unitOfWork.SaveChangesAsync();
    }
}