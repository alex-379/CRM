using CRM.Business.Services.Constants;

namespace CRM.Business.Configuration.HttpClients;

public class BaseHttpClient
{
    public HttpClient Client { get; }
    
    public BaseHttpClient(HttpClient client)
    {
        Client = client;
        Client.Timeout = new TimeSpan(NumericData.HttpClientTimeoutHour, NumericData.HttpClientTimeoutMin, NumericData.HttpClientTimeoutSec);
        Client.DefaultRequestHeaders.Clear();
    }
}

//HttpClientHandler clientHandler = new HttpClientHandler();
//clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

// Pass the handler to httpclient(from you are calling api)
//HttpClient client = new HttpClient(clientHandler);