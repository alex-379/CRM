using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Core.Constants;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Business.Services;

public class PasswordsService(SecretSettings secret) : IPasswordsService
{
    private readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

    public (string hash, string salt) HashPassword(string password)
    {
        password = $"{password}{secret.SecretPassword}";
        var salt = RandomNumberGenerator.GetBytes(PasswordSettings.KeySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            PasswordSettings.Iterations,
            _hashAlgorithm,
            PasswordSettings.KeySize);

        return (Convert.ToHexString(hash), Convert.ToHexString(salt));
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        password = $"{password}{secret.SecretPassword}";
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromHexString(salt), PasswordSettings.Iterations, _hashAlgorithm, PasswordSettings.KeySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}
