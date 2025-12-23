using System.ComponentModel.DataAnnotations;

namespace Assignment.Api.Dto.Auth;

public sealed record LoginRequest(
    [Required(ErrorMessage = "Username is required.")]
    string Username,

    [Required(ErrorMessage = "Password is required.")]
    string Password
);
