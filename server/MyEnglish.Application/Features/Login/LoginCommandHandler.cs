using MediatR;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Mappings;
using MyEnglish.Domain.ValueObjects;
using MyEnglish.Domain.Repositories;
using MyEnglish.Domain.Services;

namespace MyEnglish.Application.Features.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email
        var email = Email.Create(request.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        // Verify password
        if (!user.VerifyPassword(request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        // Check if user is active
        if (!user.IsActive)
            throw new UnauthorizedAccessException("User account is deactivated.");

        // Record login
        user.RecordLogin();

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = Domain.ValueObjects.RefreshToken.Generate();
        var userRefreshToken = user.AddRefreshToken(refreshToken);

        await _userRepository.SaveChangesAsync(cancellationToken);

        // Map to response using manual mapping
        var userDto = user.ToDto();

        return new AuthResponse(
            accessToken,
            refreshToken.Token,
            DateTime.UtcNow.Add(_jwtTokenService.AccessTokenExpiration),
            userDto);
    }
}