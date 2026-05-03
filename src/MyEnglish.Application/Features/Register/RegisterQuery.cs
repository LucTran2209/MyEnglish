using MyEnglish.Domain.Abstractions.Enums;

namespace MyEnglish.Application.Features.Register;

public sealed record RegisterQuery(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    Genders Gender,
    DateTime? DateOfBirth = null);