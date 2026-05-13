using MediatR;

namespace MyEnglish.Application.Features.Logout;

public sealed record LogoutCommand(Guid UserId, string? RefreshToken = null) : IRequest;