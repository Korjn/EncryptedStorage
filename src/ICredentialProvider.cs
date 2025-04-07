namespace Korjn.EncryptedStorage;

/// <summary>
/// Provides access to a credential stored in a JSON file with encrypted fields.
/// </summary>
public interface ICredentialJsonFileProvider
{
    /// <summary>
    /// Loads and decrypts the credential from the configured JSON file.
    /// </summary>
    /// <returns>A <see cref="Credential"/> instance with decrypted values.</returns>
    Credential Load();
}
