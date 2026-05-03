using MediatR;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Domain.Abstractions.Enums;

namespace MyEnglish.Application.Features.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    Genders Gender,
    DateTime? DateOfBirth = null) : IRequest<UserDto>;