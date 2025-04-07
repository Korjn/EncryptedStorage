namespace Korjn.EncryptedStorage;

/// <summary>
/// Indicates that the value of a property should be encrypted when persisted and decrypted when loaded.
/// </summary>
/// <remarks>
/// Used by <see cref="IEncryptedJsonFileProvider{T}"/> to determine which properties of a model should be automatically encrypted and decrypted.
/// Only applies to properties of type <see cref="string"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EncryptedAttribute : Attribute { }
