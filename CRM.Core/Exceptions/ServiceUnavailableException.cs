namespace CRM.Core.Exceptions;

public class ServiceUnavailableException(string message = DefaultMessages.ServiceUnavailableException) : Exception(message)
{
}
