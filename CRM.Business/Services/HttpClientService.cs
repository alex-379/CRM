using System.Net.Http.Headers;
using System.Text.Json;
using CRM.Business.Interfaces;
using CRM.Business.Services.Constants;
using CRM.Core;

namespace CRM.Business.Services;

public class HttpClientService<THttpClient>(THttpClient httpClient) : IHttpClientService<THttpClient> where THttpClient : IHttpClient
{
    private readonly JsonSerializerOptions _options = JsonSerializerOptionsProvider.GetJsonSerializerOptions();

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, HttpRequestMessage requestMessage)
    {
        var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, request);
        ms.Seek(0, SeekOrigin.Begin);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Data.ApplicationType));
        using var requestContent = new StreamContent(ms);
        requestMessage.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue(Data.ApplicationType);
        using var response = await httpClient.Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<TResponse>(content, _options);
        
        return result;
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri)
    {
        using var response = await httpClient.Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<TResponse>(stream, _options);

        return result;
    }
}
