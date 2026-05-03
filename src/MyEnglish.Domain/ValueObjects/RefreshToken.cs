using System.Security.Cryptography;

namespace MyEnglish.Domain.ValueObjects;

public sealed record RefreshToken
{
    public string Token { get; }
    public DateTime ExpiresAt { get; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    private RefreshToken(string token, DateTime expiresAt)
    {
        Token = token;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Generate(int expirationDays = 7)
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var token = Convert.ToBase64String(randomBytes);
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);
        
        return new RefreshToken(token, expiresAt);
    }

    public static RefreshToken Create(string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        return new RefreshToken(token, expiresAt);
    }
}