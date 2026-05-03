using MyEnglish.Domain.Abstractions;

namespace MyEnglish.Domain.Events;

public sealed record UserLoggedInEvent(
    Guid UserId,
    string Email,
    DateTime LoginTime) : DomainEvent;