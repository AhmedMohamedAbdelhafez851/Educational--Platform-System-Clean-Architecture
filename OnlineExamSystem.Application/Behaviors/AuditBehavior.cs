using MediatR;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Infrastructure.Services;
using System.Text.Json;

namespace OnlineExamSystem.Application.Behaviors
{
    public class AuditBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUser;

        public AuditBehavior(
            IAuditService auditService,
            ICurrentUserService currentUser)
        {
            _auditService = auditService;
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            // ❌ Skip Queries (only Commands)
            if (requestName.EndsWith("Query"))
                return await next();

            var userId = _currentUser.GetUserId();

            var oldRequest = JsonSerializer.Serialize(request);

            var response = await next();

            var newResponse = JsonSerializer.Serialize(response);

            await _auditService.LogAsync(
                userId,
                "EXECUTE",
                requestName,
                Guid.NewGuid().ToString(),
                oldRequest,
                newResponse
            );

            return response;
        }
    }
}