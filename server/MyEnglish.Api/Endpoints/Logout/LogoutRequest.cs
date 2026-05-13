namespace MyEnglish.Api.Endpoints.Auth;

public record LogoutRequest(string? RefreshToken = null);
