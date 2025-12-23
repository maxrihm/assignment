namespace Assignment.Api.Models.Auth;

public abstract record LoginResult
{
    public sealed record Success(string Token) : LoginResult;
    public sealed record InvalidCredentials : LoginResult;
}
