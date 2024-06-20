namespace CRM.Core.Exceptions;

public class NotFoundException(string message = DefaultMessages.NotFoundException) : Exception(message)
{
}
