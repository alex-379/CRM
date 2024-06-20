using System.Text;

namespace CRM.API.Configuration.Filters;

public static class StreamReaderFromRequest
{
    public static async Task<string> StreamReaderRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        using var streamReader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
        var requestBody = await streamReader.ReadToEndAsync();
        request.Body.Position = 0;

        return requestBody;
    }
}