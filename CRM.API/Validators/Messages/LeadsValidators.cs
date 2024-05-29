namespace CRM.API.Validators.Messages;

public static class LeadsValidators
{
    public const string Name = "You need to enter a name";
    public const string NameLength = "The length of the mail should not be longer {0} characters";
    public const string MailIncorrect = "Incorrect e-mail";
    public const string Mail = "You need to enter a mail";
    public const string MailLength = "The length of the mail should not be longer {0} characters";
    public const string Phone = "You need to enter a phone";
    public const string Address = "You need to enter an address";
    public const string AddressLength = "The length of the address should not be longer {0} characters";
    public const string BirthDate = "You need to enter a birth date";
    public const string BirthDateMin = "The date of birth must be greater than {0} year";
    public const string BirthDateMax = "The date of birth must be less than {0} year";
    public const string Status = "Incorrect lead status";
    public const string Password = "You need to enter a password";
    public const string PasswordLength = "The length of the password should not be longer {0} characters";
    public const string PasswordRule = "The password must be at least {0} characters and contain lowercase, uppercase Latin letters, digit, special character";
    public const string PhoneRule = "The phone must be at format '(999)9999999'";
}
