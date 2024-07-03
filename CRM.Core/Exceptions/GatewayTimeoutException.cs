namespace CRM.Core.Exceptions;

public class GatewayTimeoutException(string message = DefaultMessages.GatewayTimeoutException) : Exception(message)
{
}
