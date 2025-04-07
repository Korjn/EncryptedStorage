# Korjn.EncryptedStorage

🔐 A lightweight and extensible .NET library for securely loading and decrypting configuration objects from various storage backends using Microsoft.AspNetCore.DataProtection.

Includes built-in support for JSON file-based encrypted storage and is designed for future extension (memory, vaults, blobs, etc.).

---

## ✨ Features

- 🔒 Encrypts sensitive fields using ASP.NET Core Data Protection API  
- 🧩 Attribute-based encryption (`[Encrypted]`)  
- 📄 JSON file storage with auto-update on first encryption  
- 💡 Built-in support for credentials via `ICredentialJsonFileProvider`  
- 🔧 `IOptions`, named configuration, DI and snapshot support  
- 🧱 Ready for extensible storage backends (file, memory, secrets, cloud vaults)

---

## 📦 Installation

```
dotnet add package Korjn.EncryptedStorage
```

---

## 🚀 Quick Start

### 1. Define your model

```csharp
public record Credential
{
    public string? UserName { get; init; }

    [Encrypted]
    public string? Password { get; internal set; }
}
```

### 2. Create your credential file (`db.cred.json`)

```json
{
  "UserName": "admin",
  "Password": "admin123"
}
```

### 3. Register in `Program.cs`

```csharp
builder.Services.AddCredentialJsonFileProvider(options =>
{
    options.FilePath = "db.cred.json";
    options.Purpose = "MyApp/Credentials";
});
```

### 4. Inject and use

```csharp
public class MyService
{
    private readonly ICredentialJsonFileProvider provider;

    public MyService(ICredentialJsonFileProvider provider)
    {
        this.provider = provider;
    }

    public void Login()
    {
        var cred = provider.Load();
        Console.WriteLine($"User: {cred.UserName}, Password: {cred.Password}");
    }
}
```

---

## ⚙️ Configuration Options

| Property         | Description                                    | Required |
|------------------|------------------------------------------------|----------|
| `FilePath`       | Path to the JSON file                          | ✅       |
| `Purpose`        | Data protection purpose string                 | ✅       |
| `SignatureMarker`| Optional marker used to identify encrypted values | ❌    |

Default `SignatureMarker` is `"2x2"`. Encrypted strings will be prefixed with its Base64 value (e.g. `Mnh2`).

---

## 💡 Advanced: Generic Usage

You can also use the generic version of the provider with any model:

```csharp
builder.Services.AddEncryptedJsonFileProvider<ApiSecret>("MyApi", options =>
{
    options.FilePath = "secrets.json";
    options.Purpose = "MyApp/Secrets";
});
```

```csharp
public record ApiSecret
{
    public string? Name { get; init; }

    [Encrypted]
    public string? ApiKey { get; set; }
}
```

Then inject:

```csharp
public class ApiService
{
    private readonly IEncryptedJsonFileProvider<ApiSecret> provider;

    public ApiService(IEncryptedJsonFileProvider<ApiSecret> provider)
    {
        this.provider = provider;
    }

    public void Use()
    {
        var secret = provider.Load();
        Console.WriteLine($"Using {secret.Name}: {secret.ApiKey}");
    }
}
```

---

## 🔐 How Encryption Works

- Fields marked with `[Encrypted]` are encrypted using `IDataProtector`.
- Encrypted strings are prefixed with a signature (e.g., `Mnh2...`) to prevent double encryption.
- If a field is not encrypted yet, it will be encrypted on first read and the file will be updated.

---

## 📘 Interfaces

```csharp
public interface IEncryptedJsonFileProvider<T>
{
    T Load();
}

public interface ICredentialJsonFileProvider
{
    Credential Load();
}
```

---

## 🛡️ Security Notes

- Only `string` properties are supported for encryption.
- File is updated in-place only once when new encrypted values are introduced.
- Ensure `Purpose` is unique per use-case to isolate encrypted scopes.

---

## 📄 License

MIT © Korjn
