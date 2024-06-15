namespace CRM.Business.Interfaces;

public interface IHttpClientService
{
    Task<T> AddAsync<T>(T request, string url);
    Task<List<T>> GetAsync<T>();
}