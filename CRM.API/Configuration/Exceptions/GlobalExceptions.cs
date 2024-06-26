namespace CRM.API.Configuration.Exceptions;

public static class GlobalExceptions
{
    public const string LoggerError = "Exception occurred: {Message}";
    public const string InternalServerErrorException = "Server error";
    public const string ConflictException = "Conflict";
    public const string NotFoundException = "No data found for the query";
    public const string UnauthorizedException = "Authorization failed";
    public const string UnauthenticatedException = "Invalid authentication data";
    public const string ServiceUnavailableException = "Service unavailable";
    public const string GatewayTimeoutException = "Gateway timeout";
    public const string BadGatewayException = "Gateway error";
}
