namespace CRM.Core.Exceptions;

public class ConflictException(string message = DefaultMessages.ConflictException) : Exception(message)
{
}
