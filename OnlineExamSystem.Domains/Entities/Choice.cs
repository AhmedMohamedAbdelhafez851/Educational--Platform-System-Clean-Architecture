namespace OnlineExamSystem.Domains.Entities
{
    public class Choice
    {
        public int ChoiceId { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }

        public virtual Question Question { get; set; } = new();
        public virtual List<UserAnswer> UserAnswers { get; set; } = new();
    }
}