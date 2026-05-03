using MediatR;
using MyEnglish.Application.Common.DTOs;

namespace MyEnglish.Application.Features.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;