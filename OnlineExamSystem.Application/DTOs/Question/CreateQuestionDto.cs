namespace OnlineExamSystem.Application.DTOs.Question
{
    public class CreateQuestionDto
    {
        public int? QuestionId { get; set; }
        public string Title { get; set; } = "";
        public int ExamId { get; set; }
        public int CorrectChoiceIndex { get; set; } = -1;
        public List<ChoiceDto> Choices { get; set; } = new();
    }

  
}