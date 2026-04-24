using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ExamListDto>> GetAllExamsAsync(string? userId = null)
        {
            var exams = await _unitOfWork.Repository<Exam>()
                .GetAllIncludingAsync(e => e.Questions);

            var list = await exams.Select(e => new ExamListDto
            {
                ExamId = e.ExamId,
                Title = e.Title,
                QuestionsCount = e.Questions.Count
            }).ToListAsync();

            return list;
        }

        public async Task<CreateExamDto?> GetExamByIdAsync(int examId)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(examId);
            if (exam == null) return null;

            return new CreateExamDto
            {
                Title = exam.Title
            };
        }

        public async Task CreateExamAsync(CreateExamDto dto)
        {
            var exam = new Exam
            {
                Title = dto.Title,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Exam>().AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task EditExamAsync(int id, CreateExamDto dto)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(id);
            if (exam == null) return;

            exam.Title = dto.Title;

            await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteExamAsync(int examId)
        {
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(examId);
            if (exam == null) return;

            await _unitOfWork.Repository<Exam>().DeleteAsync(exam);
            await _unitOfWork.SaveChangesAsync();
        }

        // ✅ FIXED METHOD
        public async Task<TakeExamDto?> GetExamForTakingAsync(int examId)
        {
            var examQuery = await _unitOfWork.Repository<Exam>()
                .GetAllWithNestedIncludesAsync(query =>
                    query
                        .Include(e => e.Questions)
                        .ThenInclude(q => q.Choices)
                );

            var exam = await examQuery
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

            return exam;
        }
    }
}