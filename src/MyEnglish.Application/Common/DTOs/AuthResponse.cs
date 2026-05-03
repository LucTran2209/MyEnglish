namespace MyEnglish.Application.Common.DTOs;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);

public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string Gender,
    DateTime? DateOfBirth,
    int? Age,
    bool IsEmailVerified,
    bool IsActive,
    DateTime? LastLoginAt);