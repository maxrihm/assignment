namespace Assignment.Api.Interfaces;

public interface ITokenService
{
    Task<string> GetTokenAsync(string username, CancellationToken cancellationToken = default);
}
