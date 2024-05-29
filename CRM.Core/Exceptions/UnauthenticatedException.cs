namespace CRM.Core.Exceptions;

public class UnauthenticatedException(string message = "Authentication failed") : Exception(message)
{
}
