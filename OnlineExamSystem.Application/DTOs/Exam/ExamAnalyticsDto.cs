namespace OnlineExamSystem.Application.DTOs.Exam
{
    public class ExamAnalyticsDto
    {
        public int TotalStudents { get; set; }

        public int AttendedStudents { get; set; }

        public int AbsentStudents { get; set; }

        public double AverageScore { get; set; }

        public double HighestScore { get; set; }

        public double LowestScore { get; set; }

        public List<string> TopStudents { get; set; } = new();

        public List<string> WeakStudents { get; set; } = new();

        public List<QuestionAnalyticsDto> HardQuestions { get; set; } = new();
    }

    public class QuestionAnalyticsDto
    {
        public string QuestionTitle { get; set; } = "";

        public int WrongAnswersCount { get; set; }

        public double WrongPercentage { get; set; }
    }
}