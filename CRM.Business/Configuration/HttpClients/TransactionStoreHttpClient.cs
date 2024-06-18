using CRM.Business.Interfaces;

namespace CRM.Business.Configuration.HttpClients;

public class TransactionStoreHttpClient : IBaseHttpClient
{
    public HttpClient Client { get; }
    
    public TransactionStoreHttpClient(HttpClient client)
    {
        Client = client;
        Client.Timeout = new TimeSpan(HttpClientSettings.HttpClientTimeoutHour, HttpClientSettings.HttpClientTimeoutMin, HttpClientSettings.HttpClientTimeoutSec);
        Client.DefaultRequestHeaders.Clear();
        Client.BaseAddress = new Uri(HttpClientSettings.TransactionStoreApi);
    }
}
