using System.Net.Http.Headers;
using System.Text.Json;
using CRM.Business.Configuration.HttpClients;
using CRM.Business.Interfaces;

namespace CRM.Business.Services;

public class HttpClientService(BaseHttpClient httpClient) : IHttpClientService
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    
    public async Task<T> AddAsync<T>(T request, string url)
    {
        var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, request);
        ms.Seek(0, SeekOrigin.Begin);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "companies");
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        
        
        
        
        
        /*var requestContent = new StringContent(company, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("companies", requestContent);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var createdCompany = JsonSerializer.Deserialize<CompanyDto>(content, _options);
        
        
        
        
        
        
        using var response = await httpClient.Client.GetAsync("https://194.87.210.5:11000/api/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa2/balance", HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<T>>(stream, _options);

        return result;*/
        return Guid.Empty;
    }

    public async Task<List<T>> GetAsync<T>()
    {
        using var response = await httpClient.Client.GetAsync("https://194.87.210.5:11000/api/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa2/balance", HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<T>>(stream, _options);

        return result;
    }
    
    
    
    
    
    
    
    /*
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient a = transactionsClient.Client;
    
    private async Task GetAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(200000);
        
        using var response = await transactionsClient.Client.GetAsync("3fa85f64-5717-4562-b3fc-2c963f66afa1/balance", 
            HttpCompletionOption.ResponseHeadersRead,
            cancellationTokenSource.Token);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var balance = JsonSerializer.Deserialize<AccountBalanceResponse>(stream, _options);
    }*/
    
    /*private async Task CreateCompanyWithStream()
    {
        var companyForCreation = new CompanyForCreationDto
        {
            Name = "Eagle IT Ltd.",
            Country = "USA",
            Address = "Eagle IT Street 289"
        };
        var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, companyForCreation);
        ms.Seek(0, SeekOrigin.Begin);
        var request = new HttpRequestMessage(HttpMethod.Post, "companies");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using (var requestContent = new StreamContent(ms))
        {
            request.Content = requestContent;
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStreamAsync();
                var createdCompany = await JsonSerializer.DeserializeAsync<CompanyDto>(content, _options);
            }    
        }    
    }*/
}