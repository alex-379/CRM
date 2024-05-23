namespace CRM.Core.Exсeptions;

public class UnauthenticatedException(string message = "Authentication failed") : Exception(message)
{
}
