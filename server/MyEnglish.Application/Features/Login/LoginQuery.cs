namespace MyEnglish.Application.Features.Login;

public sealed record LoginQuery(
    string Email,
    string Password);