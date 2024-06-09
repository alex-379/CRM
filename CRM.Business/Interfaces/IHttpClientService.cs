namespace CRM.Business.Interfaces;

public interface IHttpClientService
{
    Task<List<T>> GetAsync<T>();
}