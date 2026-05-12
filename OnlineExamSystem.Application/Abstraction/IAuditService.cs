namespace OnlineExamSystem.Application.Abstraction
{
    public interface IAuditService
    {
        Task LogAsync(string userId, string action, string entityName, string entityId, object? oldValues, object? newValues);
    }
}