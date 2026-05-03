using MyEnglish.Application.Common.DTOs;
using MyEnglish.Domain.Entities.Users;

namespace MyEnglish.Application.Mappings;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.Gender.ToString(),
            user.DateOfBirth,
            user.Age,
            user.IsEmailVerified,
            user.IsActive,
            user.LastLoginAt);
    }
}