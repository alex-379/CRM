using CRM.Business.Interfaces;

namespace CRM.Business.Configuration.HttpClients;

public class BaseHttpClient : IHttpClient
{
    public HttpClient Client { get; }

    protected BaseHttpClient(HttpClient client)
    {
        Client = client;
        Client.Timeout = new TimeSpan(HttpClientSettings.HttpClientTimeoutHour, HttpClientSettings.HttpClientTimeoutMin, HttpClientSettings.HttpClientTimeoutSec);
        Client.DefaultRequestHeaders.Clear();
    }
}