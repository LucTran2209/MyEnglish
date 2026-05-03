using MediatR;
using MyEnglish.Domain.Repositories;

namespace MyEnglish.Application.Features.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUserRepository _userRepository;

    public LogoutCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            return; // User not found, nothing to do

        if (!string.IsNullOrEmpty(request.RefreshToken))
        {
            // Revoke specific refresh token
            user.RevokeRefreshToken(request.RefreshToken);
        }
        else
        {
            // Revoke all refresh tokens (logout from all devices)
            user.RevokeAllRefreshTokens();
        }

        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}