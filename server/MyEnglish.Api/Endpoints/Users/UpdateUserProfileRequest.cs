using MyEnglish.Domain.Abstractions.Enums;

namespace MyEnglish.Api.Endpoints.Users;

public record UpdateUserProfileRequest(
    string FirstName,
    string LastName,
    Genders Gender,
    DateTime? DateOfBirth = null);
