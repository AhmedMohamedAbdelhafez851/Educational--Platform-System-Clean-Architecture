using MediatR;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Commands.ToggleInvitation
{
    public class ToggleInvitationCommandHandler : IRequestHandler<ToggleInvitationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleInvitationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ToggleInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitation = await _unitOfWork.Repository<ExamInvitation>().GetByIdAsync(request.InvitationId);
            if (invitation == null) return false;

            invitation.IsActive = request.IsActive;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}