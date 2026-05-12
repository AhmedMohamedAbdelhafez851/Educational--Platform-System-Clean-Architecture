namespace OnlineExamSystem.Application.Abstraction
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
    }
}