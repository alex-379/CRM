using System.Text.Json;
using CRM.Business.Interfaces;
using CRM.Business.Models.Transactions.Responses;
using CRM.Business.Services.Constants;

namespace CRM.Business.Services;

public class HttpClientTransactionStoreService : IHttpClientTransactionStoreService
{
    private static readonly HttpClient _httpClient = new();
    private readonly JsonSerializerOptions _options;
    
    public HttpClientTransactionStoreService()
    {
        //_httpClient.BaseAddress = new Uri(Routes.TransactionStoreApi);
        _httpClient.Timeout = new TimeSpan(NumericData.HttpClientTimeoutHour, NumericData.HttpClientTimeoutMin, NumericData.HttpClientTimeoutSec);
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
    
    public async Task Execute()
    {
        await GetBalance();
    }

    private async Task GetBalance()
    {
        var response = await _httpClient.GetAsync("https://194.87.210.5:11000/api/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa1/balance");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var balance = JsonSerializer.Deserialize<AccountBalanceResponse>(content, _options);
    }
    
}