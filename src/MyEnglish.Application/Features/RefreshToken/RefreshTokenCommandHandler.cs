using MediatR;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Mappings;
using MyEnglish.Domain.Repositories;
using MyEnglish.Domain.Services;
using MyEnglish.Domain.ValueObjects;

namespace MyEnglish.Application.Features.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find user by refresh token
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        // Find the specific refresh token
        var refreshTokenEntity = user.RefreshTokens
            .FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // Check if user is active
        if (!user.IsActive)
            throw new UnauthorizedAccessException("User account is deactivated.");

        // Revoke the old refresh token
        user.RevokeRefreshToken(request.RefreshToken);

        // Generate new tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = Domain.ValueObjects.RefreshToken.Generate();
        var userRefreshToken = user.AddRefreshToken(newRefreshToken);

        await _userRepository.SaveChangesAsync(cancellationToken);

        // Map to response using manual mapping
        var userDto = user.ToDto();

        return new AuthResponse(
            accessToken,
            newRefreshToken.Token,
            DateTime.UtcNow.Add(_jwtTokenService.AccessTokenExpiration),
            userDto);
    }
}