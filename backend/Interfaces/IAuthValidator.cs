namespace Assignment.Api.Interfaces;

public interface IAuthValidator
{
    bool IsValidCredentials(string username, string password);
}
