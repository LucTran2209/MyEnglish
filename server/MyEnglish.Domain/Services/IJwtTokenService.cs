using MyEnglish.Domain.Entities.Users;

namespace MyEnglish.Domain.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    TimeSpan AccessTokenExpiration { get; }
}