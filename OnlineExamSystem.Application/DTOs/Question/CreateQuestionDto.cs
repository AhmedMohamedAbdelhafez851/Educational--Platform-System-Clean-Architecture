using OnlineExamSystem.Application.DTOs.Question;

namespace OnlineExamSystem.Application.DTOs.Question
{
    public class CreateQuestionDto
    {
        public int QuestionId { get; set; }
        public string Title { get; set; } = "";
        public int ExamId { get; set; }
        public string? ExamTitle { get; set; }
        public int CorrectChoiceIndex { get; set; }

        public List<ChoiceDto> Choices { get; set; } = new();
    }
}