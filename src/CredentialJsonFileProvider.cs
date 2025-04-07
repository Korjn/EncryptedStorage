namespace Korjn.EncryptedStorage;

internal class CredentialJsonFileProvider(IEncryptedJsonFileProvider<Credential> encryptedProvider) : ICredentialJsonFileProvider
{
    public Credential Load() => encryptedProvider.Load();
}
