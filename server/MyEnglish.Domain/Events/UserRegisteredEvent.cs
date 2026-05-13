using MyEnglish.Domain.Abstractions;

namespace MyEnglish.Domain.Events;

public sealed record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName) : DomainEvent;