namespace CRM.Core.Exсeptions;

public class UnauthorizedException(string message = "Access denied") : Exception(message)
{
}
