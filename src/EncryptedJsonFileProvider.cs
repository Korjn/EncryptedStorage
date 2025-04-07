using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace Korjn.EncryptedStorage;

internal class EncryptedJsonFileProvider<T> : IEncryptedJsonFileProvider<T> where T : class
{
    private readonly EncryptedJsonFileProviderOptions options;
    private readonly IDataProtector protector;
    private readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    public EncryptedJsonFileProvider(IDataProtectionProvider provider,
                                     EncryptedJsonFileProviderOptions options)
    {
        this.options = options;
        
        protector = provider.CreateProtector(options.Purpose);

        options.Signature = Convert.ToBase64String(Encoding.UTF8.GetBytes(options.SignatureMarker ?? "2x2"));
    }

    public T Load()
    {
        var filePath = Path.GetFullPath(options.FilePath);

        var result = JsonSerializer.Deserialize<T>(File.ReadAllText(filePath), jsonOptions)
                  ?? throw new InvalidOperationException("Deserialization failed");

        var encryptedProps = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttribute<EncryptedAttribute>() != null)
                    .ToArray();

        bool needRewrite = false;

        foreach (var prop in encryptedProps)
        {
            if (prop.GetValue(result) is not string value)
            {
                continue;
            }

            if (!value.StartsWith(options.Signature))
            {
                var encrypted = options.Signature + protector.Protect(value);
                prop.SetValue(result, encrypted);
                needRewrite = true;
            }

            var decrypted = protector.Unprotect(value[options.Signature.Length..]);

            prop.SetValue(result, decrypted);
        }

        if (needRewrite)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(result, jsonOptions));
        }

        return result;
    }    
}
