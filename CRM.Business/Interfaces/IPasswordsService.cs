namespace CRM.Business.Interfaces;

public interface IPasswordsService
{
    (string hash, string salt) HashPassword(string password);
    bool VerifyPassword(string password, string hash, string salt);
}