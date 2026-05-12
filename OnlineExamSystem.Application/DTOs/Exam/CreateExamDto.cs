namespace OnlineExamSystem.Application.DTOs.Exam
{
    public class CreateExamDto
    {
        public string Title { get; set; } = "";
        public int DurationInMinutes { get; set; }
        public int TotalDegree { get; set; }
    }
}