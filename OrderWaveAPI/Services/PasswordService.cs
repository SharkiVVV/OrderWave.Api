using System.Security.Cryptography;
using System.Text;

namespace OrderWaveAPI.Services;

public class PasswordService
{
    // Хешируем пароль с солью при регистрации
    // Возвращает строку вида "соль:хеш"
    public string Hash(string password)
    {
        var salt = GenerateSalt();
        var hash = ComputeHash(password, salt);
        return $"{salt}:{hash}";
    }

    public bool Verify(string password, string storedHash )
    {
        var parts= storedHash.Split(':');
        if (parts.Length != 2)
        {
            return false;
        }
        var salt = parts[0];
        var hash = parts[1];
        var computedHash  = ComputeHash(password, salt);
        return computedHash == hash;

        
    }

    private string GenerateSalt()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(bytes);
    }

    private string ComputeHash(string password, string salt)
    {
        var input = salt+password;
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

}