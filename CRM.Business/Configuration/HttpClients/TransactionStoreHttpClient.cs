namespace CRM.Business.Configuration.HttpClients;

public class TransactionStoreHttpClient : BaseHttpClient
{
    public TransactionStoreHttpClient(HttpClient client, ServicesUrlSettings settings) : base(client)
    {
        Client.BaseAddress = new Uri(settings.TransactionStore);
    }
}
