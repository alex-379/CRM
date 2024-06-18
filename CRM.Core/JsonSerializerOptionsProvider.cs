using System.Text.Json;

namespace CRM.Core;

public static class JsonSerializerOptionsProvider
{ 
    public static JsonSerializerOptions GetJsonSerializerOptions() => _jsonSerializerOptions;
    
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}