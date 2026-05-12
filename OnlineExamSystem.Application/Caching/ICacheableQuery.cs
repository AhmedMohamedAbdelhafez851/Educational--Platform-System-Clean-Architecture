namespace OnlineExamSystem.Application.Caching
{
    public interface ICacheableQuery
    {
        string CacheKey { get; }

        int SlidingExpirationInMinutes { get; }
    }
}