namespace CRM.Core.Exceptions;

public class BadRequestException(string message = "Incorrect request") : Exception(message)
{
}