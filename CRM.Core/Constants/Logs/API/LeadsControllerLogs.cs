namespace CRM.Core.Constants.Logs.API;

public static class LeadsControllerLogs
{
    public const string CreateUser = "Creating lead with mail {request.Mail}";
    public const string Login = "Lead authentication";




    public const string GetUsers = "Получаем всех пользователей";
    public const string GetUserById = "Получаем пользователя по ID {id}";
    public const string GetDevicesByUserId = "Получаем устройства по ID пользователя {userId}";
    public const string GetOrdersByUserId = "Получаем заказы по ID пользователя {userId}";


    public const string UpdateUserData = "Обновляем данные пользователя с ID {id}";
    public const string DeleteUserById = "Удаляем пользователя с ID {id}";
    public const string UpdateUserPassword = "Обновляем пароль пользователя с ID {id}";
    public const string UpdateUserMail = "Обновляем email пользователя с ID {id}";
    public const string UpdateUserRole = "Обновляем роль пользователя с ID {id}";
}
