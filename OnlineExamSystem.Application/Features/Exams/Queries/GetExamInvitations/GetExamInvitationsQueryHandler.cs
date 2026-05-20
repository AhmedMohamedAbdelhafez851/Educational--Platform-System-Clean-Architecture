using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Features.Exams.Queries.GetExamInvitations
{
    public class GetExamInvitationsQueryHandler : IRequestHandler<GetExamInvitationsQuery, List<ExamInvitationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetExamInvitationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ExamInvitationDto>> Handle(GetExamInvitationsQuery request, CancellationToken cancellationToken)
        {
            var invitations = await _unitOfWork.Repository<ExamInvitation>()
                .GetQueryable()
                .Where(i => i.ExamId == request.ExamId)
                .Include(i => i.Attempts)
                .Include(i => i.Exam)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync(cancellationToken);

            var baseUrl = "https://localhost:7059"; // Should come from configuration

            return invitations.Select(i => new ExamInvitationDto
            {
                Id = i.Id,
                ExamId = i.ExamId,
                ExamTitle = i.Exam.Title,
                Token = i.Token,
                InvitationCode = i.InvitationCode,
                InvitationLink = $"{baseUrl}/Exam/Join?code={i.InvitationCode}",
                CreatedAt = i.CreatedAt,
                ExpiresAt = i.ExpiresAt,
                MaxAttempts = i.MaxAttempts,
                IsActive = i.IsActive,
                UsedCount = i.Attempts.Count,
                Attempts = i.Attempts.Select(a => new ExamInvitationAttemptDto
                {
                    Id = a.Id,
                    StudentName = a.StudentName,
                    StudentEmail = a.StudentEmail,
                    StartedAt = a.StartedAt,
                    CompletedAt = a.CompletedAt,
                    IsCompleted = a.IsCompleted
                }).ToList()
            }).ToList();
        }
    }
}