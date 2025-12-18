using Assignment.Api.Dto.Auth;
using Assignment.Api.Models.Auth;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Extensions;

public static class LoginResultHttpExtensions
{
    public static ActionResult<LoginResponse> ToActionResult(this LoginResult result) =>
        result switch
        {
            LoginResult.Success success => new OkObjectResult(new LoginResponse(success.Token)),
            LoginResult.InvalidCredentials => Unauthorized(),
        };

    private static ActionResult<LoginResponse> Unauthorized()
    {
        return new ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = "Invalid username or password."
        })
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }
}
