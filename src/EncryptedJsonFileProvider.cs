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
        var fileJson = File.ReadAllText(options.FilePath);

        var encryptObject = JsonSerializer.Deserialize<T>(fileJson)
                  ?? throw new InvalidOperationException("Deserialization failed #1");

        // Поиск полей, помеченных как [Encrypted]
        var encryptedObjectProps = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                    .Where(p => p.GetCustomAttribute<EncryptedAttribute>() != null)
                    .ToArray();

        // Зашифровываем если есть незашифрованные поля
        foreach (var property in encryptedObjectProps)
        {
            if (property.GetValue(encryptObject) is not string propertyValue) continue;

            if (!propertyValue.StartsWith(options.Signature)) // <- не зашифровано
            {
                var encrypted = options.Signature + protector.Protect(propertyValue);
                property.SetValue(encryptObject, encrypted); // <- Перезаписываем pашифрованным                                
            }
        }

        var encryptedJson = JsonSerializer.Serialize(encryptObject, jsonOptions);

        // Сохраняем если были изменения
        if (!string.Equals(encryptedJson, fileJson))
        {
            File.WriteAllText(options.FilePath, encryptedJson);
        }

        // Загрузка объекта из зашифрованного JSON
        var result = JsonSerializer.Deserialize<T>(encryptedJson)
             ?? throw new InvalidOperationException("Deserialization failed #2");

        // Расшифровка
        foreach (var property in encryptedObjectProps)
        {
            if (property.GetValue(result) is not string propertyValue) continue;

            if (propertyValue.StartsWith(options.Signature)) // <- зашифровано
            {
                var decrypted = protector.Unprotect(propertyValue[options.Signature.Length..]);
                property.SetValue(result, decrypted); // !!! Перезаписываем расшифрованным
            }
        }

        return result;
    }
}
