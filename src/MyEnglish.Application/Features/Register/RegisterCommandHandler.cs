using MediatR;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Mappings;
using MyEnglish.Domain.Entities.Users;
using MyEnglish.Domain.Repositories;
using MyEnglish.Domain.Services;
using MyEnglish.Domain.ValueObjects;

namespace MyEnglish.Application.Features.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var email = Email.Create(request.Email);
        var userExists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        
        if (userExists)
            throw new InvalidOperationException("User with this email already exists.");

        // Create new user
        var user = User.Create(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Gender,
            request.DateOfBirth);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

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