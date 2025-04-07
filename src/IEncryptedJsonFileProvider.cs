namespace Korjn.EncryptedStorage;

/// <summary>
/// Provides loading and automatic decryption of a JSON file into an object of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The type of the model to deserialize, which may include properties marked with <see cref="EncryptedAttribute"/>.
/// </typeparam>
public interface IEncryptedJsonFileProvider<T>
{
    /// <summary>
    /// Loads the object from the JSON file and decrypts all properties marked with <see cref="EncryptedAttribute"/>.
    /// </summary>
    /// <returns>An instance of <typeparamref name="T"/> with decrypted values.</returns>
    T Load();
}