using MediatR;
using MyEnglish.Application.Common.DTOs;

namespace MyEnglish.Application.Features.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;