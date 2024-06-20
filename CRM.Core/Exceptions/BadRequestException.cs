namespace CRM.Core.Exceptions;

public class BadRequestException(string message = DefaultMessages.BadRequestException) : Exception(message)
{
}