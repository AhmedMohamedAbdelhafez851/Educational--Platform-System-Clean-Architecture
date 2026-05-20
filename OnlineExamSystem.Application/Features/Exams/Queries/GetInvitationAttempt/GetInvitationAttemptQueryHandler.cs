using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Queries.GetExamInvitations
{
    public class GetInvitationAttemptQueryHandler : IRequestHandler<GetInvitationAttemptQuery, ExamInvitationAttempt?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetInvitationAttemptQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamInvitationAttempt?> Handle(GetInvitationAttemptQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<ExamInvitationAttempt>()
                .GetQueryable()
                .Include(a => a.Submission)
                .FirstOrDefaultAsync(a => a.Id == request.AttemptId, cancellationToken);
        }
    }
}