namespace CRM.API.Configuration.Extensions;

public static class ConfigurationSettings
{
    public const string LogPath = "Serilog:WriteTo:1:Args:path";
    public const string JwtToken = "JwtToken";
    public const string ValidIssuer = "JwtToken:ValidIssuer";
    public const string ValidAudience = "JwtToken:ValidAudience";
    public const string SecretSettings = "SecretSettings";
    public const string IssuerSigningKey = "SecretSettings:SecretToken";
    public const string DatabaseSettings = "DatabaseSettings";
    public const string OpenApiTitle = "OpenApiSettings:Title";
    public const string OpenApiVersion = "OpenApiSettings:Version";
    public const string OpenApiSecurityScheme = "OpenApiSettings:SecurityScheme";
    public const string OpenApiDescription = "OpenApiSettings:Description";
    public const string OpenApiName = "OpenApiSettings:Name";
    public const string OpenApiBearerFormat = "OpenApiSettings:BearerFormat";
    public const string RabbitMqSettings = "RabbitMqSettings";
    public const string RabbitMqHost = "RabbitMqSettings:Host";
    public const string RabbitMqUserName = "RabbitMqSettings:UserName";
    public const string RabbitMqPassword = "RabbitMqSettings:Password";
    public const int TimeCansel = 60000;
}
