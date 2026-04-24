namespace OnlineExamSystem.Domains.Entities
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string Title { get; set; } = "";

        public int ExamId { get; set; }
        public int? CorrectChoiceId { get; set; }

        public Choice? CorrectChoice { get; set; }
        public Exam Exam { get; set; } = null!;

        public List<Choice> Choices { get; set; } = new();
    }
}