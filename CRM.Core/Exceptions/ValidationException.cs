namespace CRM.Core.Exceptions;

public class ValidationException(string message = DefaultMessages.ValidationException) : Exception(message)
{
}