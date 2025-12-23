using Assignment.Api.Interfaces;

namespace Assignment.Api.Services;

public sealed class TokenService : ITokenService
{
    private const string HardcodedToken = "hardcoded-token";

    public Task<string> GetTokenAsync(string username, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HardcodedToken);
    }
}
