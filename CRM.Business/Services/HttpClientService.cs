using System.Net.Http.Headers;
using System.Text.Json;
using CRM.Business.Interfaces;
using CRM.Business.Services.Constants;
using CRM.Core;
using CRM.Core.Exceptions;
using Serilog;

namespace CRM.Business.Services;

public class HttpClientService<THttpClient>(THttpClient httpClient, CancellationTokenSource cancellationTokenSource) : IHttpClientService<THttpClient> where THttpClient : IHttpClient
{
    private readonly JsonSerializerOptions _options = JsonSerializerOptionsProvider.GetJsonSerializerOptions();
    private readonly ILogger _logger = Log.ForContext<HttpClientService<THttpClient>>();
    private readonly CancellationToken _token = cancellationTokenSource.Token;
    
    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, HttpRequestMessage requestMessage)
    {
        try
        {
            var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, request, cancellationToken: _token);
            ms.Seek(0, SeekOrigin.Begin);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Data.ApplicationType));
            using var requestContent = new StreamContent(ms);
            requestMessage.Content = requestContent;
            requestContent.Headers.ContentType = new MediaTypeHeaderValue(Data.ApplicationType);
            using var response = await httpClient.Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, _token);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync(_token);
            var result = await JsonSerializer.DeserializeAsync<TResponse>(content, _options, _token);
        
            return result;
        }
        catch (OperationCanceledException ex)
        {
            throw new GatewayTimeoutException(ex.Message);
        }
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri)
    {
        try
        {
            using var response = await httpClient.Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, _token);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync(_token);
            var result = await JsonSerializer.DeserializeAsync<TResponse>(stream, _options, _token);
            
            return result;
        }
        catch (OperationCanceledException ex)
        {
            throw new GatewayTimeoutException(ex.Message);
        }
    }
}
