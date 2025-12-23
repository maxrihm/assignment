using System.Security.Cryptography;
using System.Text;

using Assignment.Api.Interfaces;
using Assignment.Api.Startup;

using Microsoft.Extensions.Options;

namespace Assignment.Api.Services;

public sealed class AuthValidator(IOptions<AuthOptions> options) : IAuthValidator
{
    private readonly AuthOptions _options = options.Value;

    public bool IsValidCredentials(string username, string password)
    {
        return FixedTimeEquals(username, _options.Username)
            && FixedTimeEquals(password, _options.Password);
    }

    private static bool FixedTimeEquals(string firstValue, string secondValue)
    {
        var firstBytes = Encoding.UTF8.GetBytes(firstValue ?? string.Empty);
        var secondBytes = Encoding.UTF8.GetBytes(secondValue ?? string.Empty);

        return firstBytes.Length == secondBytes.Length
            && CryptographicOperations.FixedTimeEquals(firstBytes, secondBytes);
    }
}
