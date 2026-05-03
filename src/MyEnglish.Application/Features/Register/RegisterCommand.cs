using MediatR;
using MyEnglish.Application.Common.DTOs;
using MyEnglish.Domain.Abstractions.Enums;

namespace MyEnglish.Application.Features.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    Genders Gender,
    DateTime? DateOfBirth = null) : IRequest<AuthResponse>;