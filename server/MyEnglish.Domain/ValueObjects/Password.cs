namespace MyEnglish.Domain.ValueObjects;

public sealed record Password
{
    public string HashedValue { get; }

    private Password(string hashedValue)
    {
        HashedValue = hashedValue;
    }

    public static Password CreateFromPlainText(string plainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            throw new ArgumentException("Password cannot be null or empty.", nameof(plainTextPassword));

        if (plainTextPassword.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters long.", nameof(plainTextPassword));

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
        return new Password(hashedPassword);
    }

    public static Password CreateFromHash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be null or empty.", nameof(hashedPassword));

        return new Password(hashedPassword);
    }

    public bool Verify(string plainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            return false;

        return BCrypt.Net.BCrypt.Verify(plainTextPassword, HashedValue);
    }
}