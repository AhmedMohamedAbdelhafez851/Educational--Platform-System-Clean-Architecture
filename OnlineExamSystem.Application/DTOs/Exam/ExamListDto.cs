namespace OnlineExamSystem.Application.DTOs.Exam
{
    public class ExamListDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = "";
        public int QuestionsCount { get; set; }
        public bool? HasSubmission { get; set; }
        public int? SubmissionId { get; set; }
        public bool? IsPassed { get; set; }
        public double? Score { get; set; }

    }
}