using MyEnglish.Application.Features.UpdateUserProfile;

namespace MyEnglish.Application.Mappings;

public static class UserCommandMappingExtensions
{
    public static UpdateUserProfileCommand ToCommand(this UpdateUserProfileQuery query, Guid userId)
    {
        return new UpdateUserProfileCommand(
            userId,
            query.FirstName,
            query.LastName,
            query.Gender,
            query.DateOfBirth);
    }
}