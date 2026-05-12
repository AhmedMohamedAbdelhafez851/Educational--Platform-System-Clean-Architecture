using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Application.Abstraction;
using OnlineExamSystem.Application.DTOs.Question;
using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public QuestionService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<List<CreateQuestionDto>> GetQuestionsByExamAsync(int examId)
        {
            var questions = await _unitOfWork.Repository<Question>()
                .GetAllWithNestedIncludesAsync(q => q.Include(x => x.Choices));

            return await questions
                .Where(q => q.ExamId == examId)
                .Select(q => new CreateQuestionDto
                {
                    QuestionId = q.QuestionId,
                    Title = q.Title,
                    ExamId = q.ExamId,
                    Choices = q.Choices.Select(c => new ChoiceDto
                    {
                        ChoiceId = c.ChoiceId,
                        Text = c.Text,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<CreateQuestionDto?> GetQuestionByIdAsync(int questionId)
        {
            var question = await (await _unitOfWork.Repository<Question>()
                .GetAllWithNestedIncludesAsync(q => q.Include(x => x.Choices)))
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);

            if (question == null) return null;

            var dto = new CreateQuestionDto
            {
                QuestionId = question.QuestionId,
                Title = question.Title,
                ExamId = question.ExamId,
                Choices = question.Choices.Select(c => new ChoiceDto
                {
                    ChoiceId = c.ChoiceId,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };

            dto.CorrectChoiceIndex = dto.Choices.FindIndex(c => c.IsCorrect);
            return dto;
        }

        public async Task AddQuestionAsync(CreateQuestionDto dto)
        {
            var question = new Question
            {
                Title = dto.Title,
                ExamId = dto.ExamId,
                Choices = dto.Choices.Select((c, i) => new Choice
                {
                    Text = c.Text,
                    IsCorrect = i == dto.CorrectChoiceIndex
                }).ToList()
            };

            await _unitOfWork.Repository<Question>().AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            if (correctChoice != null)
            {
                question.CorrectChoiceId = correctChoice.ChoiceId;
                await _unitOfWork.SaveChangesAsync();
            }

            _cacheService.Remove("AllExams");
        }

        public async Task EditQuestionAsync(CreateQuestionDto dto)
        {
            var question = await (await _unitOfWork.Repository<Question>()
                .GetAllWithNestedIncludesAsync(q => q.Include(x => x.Choices)))
                .FirstOrDefaultAsync(q => q.QuestionId == dto.QuestionId);

            if (question == null) return;

            question.Title = dto.Title;

            foreach (var old in question.Choices.ToList())
                await _unitOfWork.Repository<Choice>().DeleteAsync(old);

            question.Choices = dto.Choices.Select((c, i) => new Choice
            {
                Text = c.Text,
                IsCorrect = i == dto.CorrectChoiceIndex
            }).ToList();

            await _unitOfWork.SaveChangesAsync();

            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            if (correctChoice != null)
            {
                question.CorrectChoiceId = correctChoice.ChoiceId;
                await _unitOfWork.SaveChangesAsync();
            }

            _cacheService.Remove("AllExams");
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var question = await _unitOfWork.Repository<Question>().GetByIdAsync(questionId);
            if (question == null) return;

            await _unitOfWork.Repository<Question>().DeleteAsync(question);
            await _unitOfWork.SaveChangesAsync();

            _cacheService.Remove("AllExams");
        }
    }
}