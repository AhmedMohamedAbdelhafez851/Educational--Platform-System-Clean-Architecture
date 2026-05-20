using OnlineExamSystem.Application.DTOs.Exam;
using OnlineExamSystem.Application.DTOs.Question;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IQuestionService
    {
        Task<List<CreateQuestionDto>> GetQuestionsByExamAsync(int examId);
        Task<CreateQuestionDto?> GetQuestionByIdAsync(int questionId);
        Task AddQuestionAsync(CreateQuestionDto dto);
        Task EditQuestionAsync(CreateQuestionDto dto);
        Task DeleteQuestionAsync(int questionId);
        // Add this method to IQuestionService interface
        Task<CreateExamDto?> GetExamByIdAsync(int examId);
    }
}