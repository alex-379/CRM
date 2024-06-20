namespace CRM.Business.Interfaces;

public interface IHttpClientService<in THttpClient>
    where THttpClient : IHttpClient
{
    Task<TResponse> SendAsync<TRequest,TResponse>(TRequest request, HttpRequestMessage requestMessage);
    Task<TResponse> GetAsync<TResponse>(string uri);
}
