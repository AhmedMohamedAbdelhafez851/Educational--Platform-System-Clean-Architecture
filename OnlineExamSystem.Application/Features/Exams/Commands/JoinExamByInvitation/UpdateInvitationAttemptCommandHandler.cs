using MediatR;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Commands.JoinExamByInvitation
{
    public class UpdateInvitationAttemptCommandHandler : IRequestHandler<UpdateInvitationAttemptCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateInvitationAttemptCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateInvitationAttemptCommand request, CancellationToken cancellationToken)
        {
            var attempt = await _unitOfWork.Repository<ExamInvitationAttempt>()
                .GetByIdAsync(request.AttemptId);

            if (attempt == null)
            {
                return false;
            }

            attempt.SubmissionId = request.SubmissionId;
            attempt.CompletedAt = DateTime.UtcNow;
            attempt.IsCompleted = true;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}