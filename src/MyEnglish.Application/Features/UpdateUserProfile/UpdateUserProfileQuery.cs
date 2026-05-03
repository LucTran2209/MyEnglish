using MyEnglish.Domain.Abstractions.Enums;

namespace MyEnglish.Application.Features.UpdateUserProfile;

public sealed record UpdateUserProfileQuery(
    string FirstName,
    string LastName,
    Genders Gender,
    DateTime? DateOfBirth = null);