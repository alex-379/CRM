namespace CRM.Core.Exceptions;

public class UnauthorizedException(string message = "Access denied") : Exception(message)
{
}
