namespace CRM.Core.Constants.Logs.Business;

public static class LeadsServiceLogs
{
    public const string SetLowerRegister = "Converting mail to lowercase";
    public const string AddLead = "Сalling the repository method To create a new lead";
    public const string CompleteLead = "Create new lead with ID {id}";
    public const string AddAccount = "Сalling the repository method To create a new default RUB account for lead";
    public const string CompleteAccount = "Create new account with ID {id}";
    public const string CheckLeadByMail = "Checking the lead is in the database";
    public const string CheckUserPassword = "Verification of authentication data";




    public const string CheckUserById = "Проверяем существует ли пользователь с ID {id}";

    public const string GetUsers = "Обращаемся к методу репозитория Получение всех пользователей";
    public const string GetUserById = "Обращаемся к методу репозитория Получение пользователя по ID {id}";
    public const string UpdateUserData = "Обновляем данные пользователя с ID {id} из запроса";
    public const string UpdateUserPassword = "Обновляем пароль пользователя с ID {id} из запроса";
    public const string UpdateUserMail = "Обновляем почту пользователя с ID {id} из запроса";
    public const string UpdateUserRole = "Обновляем роль пользователя с ID {id} из запроса";
    public const string UpdateUserById = "Обращаемся к методу репозитория Обновление пользователя c ID {id}";
    public const string GetSaltByUserId = "Обращаемся к методу репозитория Получение соли пользователя с ID {id}";
    public const string DeleteSalt = "Обращаемся к методу репозитория Удаление соли пользователя с ID {id}";
    public const string UpdateSalt = "Обращаемся к методу репозитория Обновление соли для пользователя";
    public const string CompleteSalt = "Обновлена соль для пользователя с ID {id}";
    public const string SetIsDeletedUserById = "Устанавливаем IsDeleted=true для пользователя c ID {id}";
    public const string GetUserByOrderId = "Обращаемся к методу репозитория Получение пользователя по ID заказа {orderId}";
}
