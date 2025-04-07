# 📌 TODO: Encrypted Storage Improvements

## 🔄 Abstraction Layer

- Create `IEncryptedDataProvider<T>` interface
- Make `EncryptedJsonFileProvider<T>` implement it
- Allow multiple backends: file, memory, environment, vault

**Interface `IEncryptedDataProvider<T>`** — the next level of abstraction above `IEncryptedJsonFileProvider<T>`.  
It is needed to support different storage types: JSON, memory, environment, cloud, etc., while still automatically encrypting/decrypting fields marked with `[Encrypted]`.

## Features to Add

- `Save(T)` method
- Async support: `LoadAsync`, `SaveAsync`
- Auto-refresh / file watcher

### 🎯 Goal of `IEncryptedDataProvider<T>`

Unify access to encrypted data regardless of the storage source:

| Provider Type             | Implementation                        |
|--------------------------|----------------------------------------|
| JSON file                | `EncryptedJsonFileProvider<T>`         |
| In-memory                | `EncryptedMemoryProvider<T>`           |
| Environment variables    | `EncryptedEnvironmentProvider<T>`      |
| Cloud (Vault/S3/Blob)    | `EncryptedVaultProvider<T>`            |

---

### 🧪 Example interface

```csharp
namespace Korjn.Security.EncryptedStorage;

/// <summary>
/// Represents an encrypted data provider that can load and optionally save an instance of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type containing encrypted fields.</typeparam>
public interface IEncryptedDataProvider<T>
{
    /// <summary>
    /// Loads the object and decrypts fields marked with <see cref="EncryptedAttribute"/>.
    /// </summary>
    /// <returns>The decrypted object of type <typeparamref name="T"/>.</returns>
    T Load();    
}
```

---

### ✅ Benefits: one interface — multiple backends

You can use it like this:

```csharp
public class MyService(IEncryptedDataProvider<Credential> provider)
{
    public void Use()
    {
        var cred = provider.Load();
        Console.WriteLine(cred.Password);
    }
}
```

---

### 🧩 Planned Implementations

- [ ] `EncryptedJsonFileProvider<T>` — JSON file storage
- [ ] `EncryptedMemoryProvider<T>` — in-memory object
- [ ] `EncryptedEnvironmentProvider<T>` — from environment variables
- [ ] `EncryptedVaultProvider<T>` — cloud vault (Azure Key Vault, HashiCorp, etc.)
- [ ] Update README and DI extensions