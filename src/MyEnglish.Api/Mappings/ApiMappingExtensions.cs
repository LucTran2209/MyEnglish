using MyEnglish.Api.Endpoints.Auth;
using MyEnglish.Api.Endpoints.Login;
using MyEnglish.Api.Endpoints.Register;
using MyEnglish.Api.Endpoints.Users;
using MyEnglish.Application.Features.Login;
using MyEnglish.Application.Features.RefreshToken;
using MyEnglish.Application.Features.Register;
using MyEnglish.Application.Features.UpdateUserProfile;

namespace MyEnglish.Api.Mappings;

public static class ApiMappingExtensions
{
    public static RegisterQuery ToQuery(this RegisterRequest request)
    {
        return new RegisterQuery(
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.FirstName,
            request.LastName,
            request.Gender,
            request.DateOfBirth);
    }

    public static LoginQuery ToQuery(this LoginRequest request)
    {
        return new LoginQuery(request.Email, request.Password);
    }

    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request)
    {
        return new RefreshTokenCommand(request.RefreshToken);
    }   

    public static UpdateUserProfileCommand ToCommand(this UpdateUserProfileRequest request, Guid userId)
    {
        return new UpdateUserProfileCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.Gender,
            request.DateOfBirth);
    }
}