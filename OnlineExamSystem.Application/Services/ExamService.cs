using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExamService> _logger;

        public ExamService(IUnitOfWork unitOfWork,
                           ILogger<ExamService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<ExamListDto>> GetAllExamsAsync(string? userId = null)
        {
            _logger.LogInformation("Fetching all exams for user {UserId}", userId);

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
            _logger.LogInformation("Fetching exam by id {ExamId}", examId);

            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(examId);

            if (exam == null)
            {
                _logger.LogWarning("Exam {ExamId} not found", examId);
                return null;
            }

            return new CreateExamDto
            {
                Title = exam.Title
            };
        }

        public async Task CreateExamAsync(CreateExamDto dto)
        {
            _logger.LogInformation("Creating exam with title {Title}", dto.Title);

            var exam = new Exam
            {
                Title = dto.Title,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Exam>().AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Exam created successfully");
        }

        public async Task EditExamAsync(int id, CreateExamDto dto)
        {
            _logger.LogInformation("Editing exam {ExamId}", id);

            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(id);

            if (exam == null)
            {
                _logger.LogWarning("Exam {ExamId} not found", id);
                return;
            }

            exam.Title = dto.Title;

            await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Exam {ExamId} updated successfully", id);
        }

        public async Task DeleteExamAsync(int examId)
        {
            _logger.LogInformation("Deleting exam {ExamId}", examId);

            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(examId);

            if (exam == null)
            {
                _logger.LogWarning("Exam {ExamId} not found", examId);
                return;
            }

            await _unitOfWork.Repository<Exam>().DeleteAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Exam {ExamId} deleted successfully", examId);
        }

        public async Task<TakeExamDto?> GetExamForTakingAsync(int examId)
        {
            _logger.LogInformation("Fetching exam {ExamId} for taking", examId);

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

            if (exam == null)
            {
                _logger.LogWarning("Exam {ExamId} not found for taking", examId);
            }

            return exam;
        }
    }
}