using Assignment.Api.Dto.Auth;
using Assignment.Api.Interfaces;
using Assignment.Api.Models.Auth;

namespace Assignment.Api.Services;

public sealed class AuthService(
    ITokenService tokenService,
    IAuthValidator authValidator) : IAuthService
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly IAuthValidator _authValidator = authValidator;

    public async Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username;
        var password = request.Password;

        if (!_authValidator.IsValidCredentials(username, password))
        {
            return new LoginResult.InvalidCredentials();
        }

        var token = await _tokenService.GetTokenAsync(username, cancellationToken);
        return new LoginResult.Success(token);
    }
}


