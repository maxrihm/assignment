namespace Assignment.Api.Startup;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public required string Username { get; init; }
    public required string Password { get; init; }
}


