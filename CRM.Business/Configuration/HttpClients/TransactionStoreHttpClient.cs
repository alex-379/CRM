namespace CRM.Business.Configuration.HttpClients;

public class TransactionStoreHttpClient : BaseHttpClient
{
    public TransactionStoreHttpClient(HttpClient client) : base(client)
    {
        Client.BaseAddress = new Uri(HttpClientSettings.TransactionStoreApi);
    }
}
