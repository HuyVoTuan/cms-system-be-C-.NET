namespace Dummy.Infrastructure.Services
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, TimeSpan expirationTime);
        object RemoveData(string key);
    }
}
