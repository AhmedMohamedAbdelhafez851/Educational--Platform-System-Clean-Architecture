namespace OnlineExamSystem.Application.DTOs.Exam
{
    public class TakeExamDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = "";
        public int DurationInMinutes { get; set; } // Add this property
        public int TotalDegree { get; set; } // Add this property
        public List<QuestionTakeDto> Questions { get; set; } = new();
    }

    public class QuestionTakeDto
    {
        public int QuestionId { get; set; }
        public string Title { get; set; } = "";
        public List<ChoiceTakeDto> Choices { get; set; } = new();
    }

    public class ChoiceTakeDto
    {
        public int ChoiceId { get; set; }
        public string Text { get; set; } = "";
    }
}