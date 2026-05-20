using OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Application.Abstraction
{
    public interface IExamSubmissionService
    {
        Task<List<ExamSubmission>> GetUserSubmissionsAsync(string userId);
        Task<ExamSubmission?> GetSubmissionDetailsAsync(int submissionId, string? userId = null);
        Task<ExamSubmission> SubmitExamAsync(string userId, int examId, Dictionary<int, int> answers, string studentName = "", string studentEmail = "", string studentId = "");
    }
}