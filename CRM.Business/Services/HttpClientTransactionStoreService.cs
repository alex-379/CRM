using CRM.Business.Interfaces;
using CRM.Business.Services.Constants;

namespace CRM.Business.Services;

public class HttpClientTransactionStoreService : IHttpClientTransactionStoreService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    public HttpClientTransactionStoreService()
    {
        _httpClient.BaseAddress = new Uri(Routes.TransactionStoreApi);
        _httpClient.Timeout = new TimeSpan(NumericData.HttpClientTimeoutHour, NumericData.HttpClientTimeoutMin, NumericData.HttpClientTimeoutSec);
    }
    
    public async Task Execute()
    {
    }
}