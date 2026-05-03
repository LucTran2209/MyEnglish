using MediatR;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Application.Mappings;
using MyEnglish.Domain.Repositories;

namespace MyEnglish.Application.Features.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfileCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            throw new InvalidOperationException("User not found.");

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.DateOfBirth);

        await _userRepository.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}