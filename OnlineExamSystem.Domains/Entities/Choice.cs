namespace OnlineExamSystem.Domains.Entities
{
    public class Choice
    {
        public int ChoiceId { get; set; }
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }

        // Navigation properties
        public Question Question { get; set; } = null!;
    }
}