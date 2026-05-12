namespace OnlineExamSystem.Domains.Entities
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string Title { get; set; } = "";
        public int ExamId { get; set; }
        public int? CorrectChoiceId { get; set; }

        // Navigation properties
        public Exam Exam { get; set; } = null!;
        public Choice? CorrectChoice { get; set; }
        public List<Choice> Choices { get; set; } = new();
    }
}