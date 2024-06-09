using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Core.Constants;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Business.Services;

public static class PasswordsService
{
    private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

    public static (string hash, string salt) HashPassword(string password, string secret)
    {
        password = $"{password}{secret}";
        var salt = RandomNumberGenerator.GetBytes(PasswordSettings.KeySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            PasswordSettings.Iterations,
            _hashAlgorithm,
            PasswordSettings.KeySize);

        return (Convert.ToHexString(hash), Convert.ToHexString(salt));
    }

    public static bool VerifyPassword(string password, string secret, string hash, string salt)
    {
        password = $"{password}{secret}";
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromHexString(salt), PasswordSettings.Iterations, _hashAlgorithm, PasswordSettings.KeySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}
