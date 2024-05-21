namespace CRM.Core.Constants.Exceptions;

public static class GlobalExceptions
{
    public const string LoggerError = "Exception occurred: {Message}";
    public const string InternalServerErrorException = "Ошибка сервера";
    public const string ConflictException = "Конфликт";
    public const string NotFoundException = "Не найдены данные по запросу";
    public const string UnauthorizedException = "Не пройдена авторизация";
    public const string UnauthenticatedException = "Неверные аутентификационные данные";
}
