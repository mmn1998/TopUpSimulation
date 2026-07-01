namespace SIMA.Framework.Infrastructure.RestfulClient;

public interface IRestfulClient
{
    Task<TResult> PostAsync<TResult, TRequest>(string actionUrl, TRequest request, Dictionary<string, string>? headers = null) where TResult : class;
}
