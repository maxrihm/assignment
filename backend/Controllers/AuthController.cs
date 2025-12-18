using Assignment.Api.Dto.Auth;
using Assignment.Api.Extensions;
using Assignment.Api.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var loginResult = await _authService.LoginAsync(request, cancellationToken);
        return loginResult.ToActionResult();
    }
}
