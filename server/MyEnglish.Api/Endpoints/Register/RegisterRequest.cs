using MyEnglish.Domain.Abstractions.Enums;

namespace MyEnglish.Api.Endpoints.Register;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    Genders Gender,
    DateTime? DateOfBirth = null);