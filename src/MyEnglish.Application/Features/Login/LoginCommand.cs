using MediatR;
using MyEnglish.Application.Common.DTOs;

namespace MyEnglish.Application.Features.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;