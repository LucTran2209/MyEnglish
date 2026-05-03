using MyEnglish.Domain.Abstractions.IEntities;
using MyEnglish.Domain.ValueObjects;

namespace MyEnglish.Domain.Entities.Users;

public class UserRefreshToken : EntityAuditBase
{
    // Private constructor for EF Core
    private UserRefreshToken() { }

    private UserRefreshToken(Guid userId, RefreshToken refreshToken)
    {
        UserId = userId;
        Token = refreshToken.Token;
        ExpiresAt = refreshToken.ExpiresAt;
        IsRevoked = false;
        RevokedAt = null;
    }

    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;

    // Computed properties
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Factory method
    public static UserRefreshToken Create(Guid userId, RefreshToken refreshToken)
    {
        return new UserRefreshToken(userId, refreshToken);
    }

    // Domain methods
    public void Revoke()
    {
        if (IsRevoked) return;

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        LastModifiedDate = DateTimeOffset.UtcNow;
    }
}