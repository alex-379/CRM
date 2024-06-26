namespace CRM.Core.Exceptions;

public static class DefaultMessages
{
    public const string BadRequestException = "Incorrect request";
    public const string ConflictException = "Conflict";
    public const string ServiceUnavailableException = "No Connection with the database";
    public const string NotFoundException = "Object not found";
    public const string UnauthenticatedException = "Authentication failed";
    public const string UnauthorizedException = "Access denied";
    public const string GatewayTimeoutException = "The gateway is not responding";
    public const string BadGatewayException = "The gateway returned an error";
}
