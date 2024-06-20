namespace CRM.Core.Exceptions;

public class UnauthorizedException(string message = DefaultMessages.UnauthorizedException) : Exception(message)
{
}
