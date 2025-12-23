using Assignment.Api.Dto.Auth;
using Assignment.Api.Models.Auth;

namespace Assignment.Api.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}


