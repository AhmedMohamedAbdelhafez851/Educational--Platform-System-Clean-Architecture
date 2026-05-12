namespace OnlineExamSystem.Application.Abstraction
{
    public interface ICurrentUserService
    {
        string GetUserId();
        string GetUserName();
        bool IsAuthenticated();
    }
}