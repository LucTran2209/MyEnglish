using MyEnglish.Application.Features.Login;
using MyEnglish.Application.Features.RefreshToken;
using MyEnglish.Application.Features.Register;

namespace MyEnglish.Application.Mappings;

public static class AuthMappingExtensions
{
    public static RegisterCommand ToCommand(this RegisterQuery query)
    {
        return new RegisterCommand(
            query.Email,
            query.Password,
            query.ConfirmPassword,
            query.FirstName,
            query.LastName,
            query.Gender,
            query.DateOfBirth);
    }

    public static LoginCommand ToCommand(this LoginQuery query)
    {
        return new LoginCommand(query.Email, query.Password);
    }

    public static RefreshTokenCommand ToCommand(this RefreshTokenQuery query)
    {
        return new RefreshTokenCommand(query.RefreshToken);
    }
}