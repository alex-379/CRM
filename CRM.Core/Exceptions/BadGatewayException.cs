namespace CRM.Core.Exceptions;

public class BadGatewayException(string message = DefaultMessages.BadGatewayException) : Exception(message)
{
}