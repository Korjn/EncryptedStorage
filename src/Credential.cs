namespace Korjn.EncryptedStorage;

/// <summary>
/// Represents a simple user credential with optional username and an encrypted password.
/// </summary>
public record Credential
{
    /// <summary>
    /// Gets the user name or login identifier.
    /// </summary>
    /// <remarks>
    /// This value is stored as plain text.
    /// </remarks>
    public string? UserName { get; init; }

    /// <summary>
    /// Gets or sets the user password.
    /// </summary>
    /// <remarks>
    /// The value is expected to be encrypted and decrypted automatically by <see cref="IEncryptedJsonFileProvider{T}"/>.
    /// </remarks>
    [Encrypted]    
    public string? Password { get; internal set; }
}