using OnlineExamSystem.Application.DTOs.Exam;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IExamService
    {
        Task<List<ExamListDto>> GetAllExamsAsync(string? userId = null);
        Task<CreateExamDto?> GetExamByIdAsync(int examId);
        Task CreateExamAsync(CreateExamDto dto);
        Task EditExamAsync(int id, CreateExamDto dto);
        Task DeleteExamAsync(int examId);
        Task<TakeExamDto?> GetExamForTakingAsync(int examId);
    }
}