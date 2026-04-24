namespace OnlineExamSystem.Application.DTOs.Submission
{
    public class ExamResultDto
    {
        public int SubmissionId { get; set; }
        public string ExamTitle { get; set; } = "";
        public DateTime SubmissionDate { get; set; }
        public int? CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public double? Score { get; set; }
        public bool? IsPassed { get; set; }

        public List<AnswerDetailDto> Answers { get; set; } = new();
    }

    public class AnswerDetailDto
    {
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; } = "";
        public string UserAnswer { get; set; } = "";
        public string CorrectAnswer { get; set; } = "";
        public bool IsCorrect { get; set; }
    }
}