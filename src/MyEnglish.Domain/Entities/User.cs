using MyEnglish.Domain.Abstractions;
using MyEnglish.Domain.Abstractions.IEntities;
using MyEnglish.Domain.Abstractions.Enums;
using MyEnglish.Domain.Events;
using MyEnglish.Domain.Extensions;
using MyEnglish.Domain.ValueObjects;

namespace MyEnglish.Domain.Entities.Users;

public class User : EntityAuditBase
{
    private readonly List<IDomainEvent> _domainEvents = new();

    // Private constructor for EF Core
    private User() { }

    private User(
        Email email,
        Password password,
        string firstName,
        string lastName,
        Genders gender,
        DateTime? dateOfBirth = null)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        IsEmailVerified = false;
        IsActive = true;
        LastLoginAt = null;
        RefreshTokens = new List<UserRefreshToken>();

        // Raise domain event
        _domainEvents.Add(new UserRegisteredEvent(Id, email, firstName, lastName));
    }

    // Properties with private setters for encapsulation
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Genders Gender { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Navigation properties
    public ICollection<UserRefreshToken> RefreshTokens { get; private set; } = new List<UserRefreshToken>();

    // Computed properties
    public string FullName => $"{FirstName} {LastName}";
    public int? Age => DateOfBirth?.CalculateAge();

    // Domain events
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Factory method
    public static User Create(
        string email,
        string password,
        string firstName,
        string lastName,
        Genders gender,
        DateTime? dateOfBirth = null)
    {
        var emailVO = Email.Create(email);
        var passwordVO = Password.CreateFromPlainText(password);

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));

        if (dateOfBirth.HasValue && dateOfBirth.Value > DateTime.Today)
            throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

        return new User(emailVO, passwordVO, firstName.Trim(), lastName.Trim(), gender, dateOfBirth);
    }

    // Domain methods
    public bool VerifyPassword(string plainTextPassword)
    {
        return Password.Verify(plainTextPassword);
    }

    public void ChangePassword(string newPassword)
    {
        Password = Password.CreateFromPlainText(newPassword);
        LastModifiedDate = DateTimeOffset.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, Genders gender, DateTime? dateOfBirth = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));

        if (dateOfBirth.HasValue && dateOfBirth.Value > DateTime.Today)
            throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Gender = gender;
        DateOfBirth = dateOfBirth;
        LastModifiedDate = DateTimeOffset.UtcNow;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        LastModifiedDate = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        LastModifiedDate = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        LastModifiedDate = DateTimeOffset.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        LastModifiedDate = DateTimeOffset.UtcNow;
        
        _domainEvents.Add(new UserLoggedInEvent(Id, Email, LastLoginAt.Value));
    }

    public UserRefreshToken AddRefreshToken(RefreshToken refreshToken)
    {
        // Remove expired tokens
        var expiredTokens = RefreshTokens.Where(rt => rt.IsExpired).ToList();
        foreach (var expiredToken in expiredTokens)
        {
            RefreshTokens.Remove(expiredToken);
        }

        var userRefreshToken = UserRefreshToken.Create(Id, refreshToken);
        RefreshTokens.Add(userRefreshToken);
        
        return userRefreshToken;
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = RefreshTokens.FirstOrDefault(rt => rt.Token == token);
        if (refreshToken != null)
        {
            refreshToken.Revoke();
        }
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in RefreshTokens.Where(rt => !rt.IsRevoked))
        {
            token.Revoke();
        }
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
