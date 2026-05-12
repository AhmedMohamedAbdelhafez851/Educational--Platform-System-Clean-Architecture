using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUser;

        public ExamService(IUnitOfWork unitOfWork, IAuditService auditService, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _currentUser = currentUser;
        }

        public async Task<List<ExamListDto>> GetAllExamsAsync(string? userId = null)
        {
            var exams = await _unitOfWork.Repository<Exam>()
                .GetAllIncludingAsync(e => e.Questions);

            return await exams.Select(e => new ExamListDto
            {
                ExamId = e.ExamId,
                Title = e.Title,
                DurationInMinutes = e.DurationInMinutes,
                TotalDegree = e.TotalDegree,
                QuestionsCount = e.Questions.Count
            }).ToListAsync();
        }

        public async Task<CreateExamDto?> GetExamByIdAsync(int examId)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(examId);
            if (exam == null) return null;

            return new CreateExamDto
            {
                Title = exam.Title,
                DurationInMinutes = exam.DurationInMinutes,
                TotalDegree = exam.TotalDegree
            };
        }

        public async Task CreateExamAsync(CreateExamDto dto)
        {
            var exam = new Exam
            {
                Title = dto.Title,
                DurationInMinutes = dto.DurationInMinutes,
                TotalDegree = dto.TotalDegree,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Exam>().AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogAsync(_currentUser.GetUserId(), "CREATE", "Exam", exam.ExamId.ToString(), null, new { exam.Title });
        }

        public async Task EditExamAsync(int id, CreateExamDto dto)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(id);
            if (exam == null) return;

            var oldValues = new { exam.Title, exam.DurationInMinutes, exam.TotalDegree };

            exam.Title = dto.Title;
            exam.DurationInMinutes = dto.DurationInMinutes;
            exam.TotalDegree = dto.TotalDegree;

            await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogAsync(_currentUser.GetUserId(), "UPDATE", "Exam", exam.ExamId.ToString(), oldValues, new { exam.Title });
        }

        public async Task DeleteExamAsync(int examId)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(examId);
            if (exam == null) return;

            var oldValues = new { exam.Title };

            await _unitOfWork.Repository<Exam>().DeleteAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogAsync(_currentUser.GetUserId(), "DELETE", "Exam", exam.ExamId.ToString(), oldValues, null);
        }

        public async Task<TakeExamDto?> GetExamForTakingAsync(int examId)
        {
            var examQuery = await _unitOfWork.Repository<Exam>()
                .GetAllWithNestedIncludesAsync(query => query
                    .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices));

            return await examQuery
                .Where(e => e.ExamId == examId)
                .Select(e => new TakeExamDto
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    Questions = e.Questions.Select(q => new QuestionTakeDto
                    {
                        QuestionId = q.QuestionId,
                        Title = q.Title,
                        Choices = q.Choices.Select(c => new ChoiceTakeDto
                        {
                            ChoiceId = c.ChoiceId,
                            Text = c.Text
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}