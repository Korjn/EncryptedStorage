using System.ComponentModel.DataAnnotations;

namespace Korjn.EncryptedStorage;

/// <summary>
/// Configuration options for <see cref="IEncryptedJsonFileProvider{T}"/>.
/// </summary>
public class EncryptedJsonFileProviderOptions
{
    /// <summary>
    /// Gets or sets the absolute or relative path to the JSON file containing the data.
    /// </summary>
    /// <remarks>
    /// This file will be automatically read and updated if encrypted values are missing or not signed.
    /// </remarks>
    [Required]
    public required string FilePath { get; set; }

    /// <summary>
    /// Gets or sets the marker prefix used to identify already encrypted values.
    /// </summary>
    /// <remarks>
    /// If not specified, the default marker <c>"2x2"</c> is used.
    /// </remarks>
    public string? SignatureMarker { get; set; }

    /// <summary>
    /// Gets or sets the unique purpose string used to create a data protection scope.
    /// </summary>
    /// <remarks>
    /// This is used to isolate protected values and must match when decrypting.
    /// </remarks>
    [Required]
    public required string Purpose { get; set; }

    /// <summary>
    /// Gets or sets the Base64-encoded signature prefix that is prepended to encrypted values.
    /// </summary>
    /// <remarks>
    /// This value is generated internally based on <see cref="SignatureMarker"/>.
    /// </remarks>
    internal string Signature { get; set; } = default!;
}