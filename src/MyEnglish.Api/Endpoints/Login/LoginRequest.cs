namespace MyEnglish.Api.Endpoints.Login;

public sealed record LoginRequest(
    string Email,
    string Password);