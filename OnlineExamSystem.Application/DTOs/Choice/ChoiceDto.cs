namespace OnlineExamSystem.Application.DTOs.Question
{
    public class ChoiceDto
    {
        public int ChoiceId { get; set; }
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }
    }
}