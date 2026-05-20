using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Commands.JoinExamByInvitation
{
    public class JoinExamByInvitationCommandHandler : IRequestHandler<JoinExamByInvitationCommand, JoinExamResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public JoinExamByInvitationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<JoinExamResultDto> Handle(JoinExamByInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitation = await _unitOfWork.Repository<ExamInvitation>()
                .GetQueryable()
                .Include(i => i.Exam)
                .FirstOrDefaultAsync(i => i.InvitationCode == request.JoinInfo.InvitationCode && i.IsActive, cancellationToken);

            if (invitation == null)
            {
                return new JoinExamResultDto
                {
                    Success = false,
                    Message = "Invalid or expired invitation code."
                };
            }

            if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt < DateTime.UtcNow)
            {
                return new JoinExamResultDto
                {
                    Success = false,
                    Message = "This invitation has expired."
                };
            }

            var attemptsCount = await _unitOfWork.Repository<ExamInvitationAttempt>()
                .GetCountAsync(a => a.InvitationId == invitation.Id && a.StudentName == request.JoinInfo.StudentName);

            if (attemptsCount >= invitation.MaxAttempts)
            {
                return new JoinExamResultDto
                {
                    Success = false,
                    Message = $"You have reached the maximum allowed attempts ({invitation.MaxAttempts})."
                };
            }

            var attempt = new ExamInvitationAttempt
            {
                InvitationId = invitation.Id,
                StudentName = request.JoinInfo.StudentName,
                StudentEmail = request.JoinInfo.StudentEmail,
                StudentId = request.JoinInfo.StudentId,
                StartedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            await _unitOfWork.Repository<ExamInvitationAttempt>().AddAsync(attempt);
            await _unitOfWork.SaveChangesAsync();

            // Store attempt info in TempData for the exam taking page
            var redirectUrl = $"/UserExam/TakeExam/{invitation.ExamId}?attemptId={attempt.Id}";

            return new JoinExamResultDto
            {
                Success = true,
                ExamId = invitation.ExamId,
                AttemptId = attempt.Id,
                RedirectUrl = redirectUrl
            };
        }
    }
}