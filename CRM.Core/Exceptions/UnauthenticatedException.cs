namespace CRM.Core.Exceptions;

public class UnauthenticatedException(string message = DefaultMessages.UnauthenticatedException) : Exception(message)
{
}
