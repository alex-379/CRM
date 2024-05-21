namespace CRM.Core.Constants.Exceptions.Business;

public class LeadsServiceExceptions
{
    public const string NotFoundException = "Пользователь с ID {0} не найден";
    public const string NotFoundExceptionMail = "Пользователь с почтой {0} не найден";
    public const string ConflictException = "Такой e-mail уже существует";
}
