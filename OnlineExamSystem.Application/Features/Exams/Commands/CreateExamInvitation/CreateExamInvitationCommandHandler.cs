using MediatR;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Commands.CreateExamInvitation
{
    public class CreateExamInvitationCommandHandler : IRequestHandler<CreateExamInvitationCommand, ExamInvitationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateExamInvitationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ExamInvitationDto> Handle(CreateExamInvitationCommand request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.ExamId);
            if (exam == null) throw new Exception("Exam not found");

            var token = Guid.NewGuid().ToString("N");
            var shortCode = GenerateShortCode();

            var invitation = new ExamInvitation
            {
                ExamId = request.ExamId,
                Token = token,
                InvitationCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpiresAt,
                MaxAttempts = request.MaxAttempts,
                IsActive = true,
                CreatedByUserId = _currentUser.GetUserId()
            };

            await _unitOfWork.Repository<ExamInvitation>().AddAsync(invitation);
            await _unitOfWork.SaveChangesAsync();

            // Generate full link
            // In CreateExamInvitationCommandHandler.cs, change this line:
            var baseUrl = "https://localhost:7059"; // Or get from configuration
            var invitationLink = $"{baseUrl}/UserExam/Join?code={shortCode}";  // Changed from /Exam/Join to /UserExam/Join

            return new ExamInvitationDto
            {
                Id = invitation.Id,
                ExamId = invitation.ExamId,
                ExamTitle = exam.Title,
                Token = invitation.Token,
                InvitationCode = invitation.InvitationCode,
                InvitationLink = invitationLink,
                CreatedAt = invitation.CreatedAt,
                ExpiresAt = invitation.ExpiresAt,
                MaxAttempts = invitation.MaxAttempts,
                IsActive = invitation.IsActive,
                UsedCount = 0
            };
        }

        private string GenerateShortCode()
        {
            // Generate 6-character alphanumeric code
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}