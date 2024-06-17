using CRM.Business.Interfaces;

namespace CRM.Business.Configuration.HttpClients;

public class TransactionStoreHttpClient : IBaseHttpClient
{
    public HttpClient Client { get; }
    
    public TransactionStoreHttpClient(HttpClient client)
    {
        Client = client;
        Client.Timeout = new TimeSpan(Settings.HttpClientTimeoutHour, Settings.HttpClientTimeoutMin, Settings.HttpClientTimeoutSec);
        Client.DefaultRequestHeaders.Clear();
        Client.BaseAddress = new Uri(Settings.TransactionStoreApi);
    }
}
