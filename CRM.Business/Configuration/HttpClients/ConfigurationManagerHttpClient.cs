namespace CRM.Business.Configuration.HttpClients;

public class ConfigurationManagerHttpClient : BaseHttpClient
{
    public ConfigurationManagerHttpClient(HttpClient client) : base(client)
    {
        Client.BaseAddress = new Uri(HttpClientSettings.ConfigurationManagerApi);
    }
}