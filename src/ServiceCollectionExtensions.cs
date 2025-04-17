using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Korjn.EncryptedStorage.DependencyInjection;

/// <summary>
/// Provides extension methods for registering <see cref="IEncryptedJsonFileProvider{T}"/> and related services into the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="IEncryptedJsonFileProvider{T}"/> with a named <see cref="EncryptedJsonFileProviderOptions"/> configuration.
    /// </summary>
    /// <typeparam name="T">The type that contains properties marked with <see cref="EncryptedAttribute"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
    /// <param name="name">The name used for options binding and DI resolution.</param>
    /// <param name="configureOptions">The action to configure <see cref="EncryptedJsonFileProviderOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddEncryptedJsonFileProvider<T>(
        this IServiceCollection services,
        string name,
        Action<EncryptedJsonFileProviderOptions> configureOptions)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        ArgumentNullException.ThrowIfNull(name);

        services.AddOptions<EncryptedJsonFileProviderOptions>(name)
                .Configure(configureOptions);

        services.TryAddKeyedSingleton<IEncryptedJsonFileProvider<T>>(name, (sp, _) =>
        {
            var optionsAccessor = sp.GetRequiredService<IOptionsMonitor<EncryptedJsonFileProviderOptions>>();
            var provider = sp.GetRequiredService<IDataProtectionProvider>();

            var options = optionsAccessor.Get(name?.ToString());

            return new EncryptedJsonFileProvider<T>(provider, options);
        });

        return services;
    }

    /// <summary>
    /// Registers an <see cref="IEncryptedJsonFileProvider{T}"/> using the default (unnamed) options configuration.
    /// </summary>
    /// <typeparam name="T">The type that contains properties marked with <see cref="EncryptedAttribute"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
    /// <param name="configureOptions">The action to configure <see cref="EncryptedJsonFileProviderOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddEncryptedJsonFileProvider<T>(
        this IServiceCollection services,
        Action<EncryptedJsonFileProviderOptions> configureOptions)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);        

        services.TryAddTransient<IEncryptedJsonFileProvider<T>>(sp =>
        {
            var optionsAccessor = sp.GetRequiredService<IOptions<EncryptedJsonFileProviderOptions>>();
            var provider = sp.GetRequiredService<IDataProtectionProvider>();            

            return new EncryptedJsonFileProvider<T>(provider, optionsAccessor.Value);
        });

        services.Configure(configureOptions);

        return services;
    }

    /// <summary>
    /// Registers <see cref="ICredentialJsonFileProvider"/> with a named <see cref="EncryptedJsonFileProviderOptions"/> configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
    /// <param name="name">The name used for options binding and DI resolution.</param>
    /// <param name="configureOptions">The action to configure <see cref="EncryptedJsonFileProviderOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddCredentialJsonFileProvider(
        this IServiceCollection services,
        string name,
        Action<EncryptedJsonFileProviderOptions> configureOptions)
    {
        services.AddEncryptedJsonFileProvider<Credential>(name, configureOptions);
        services.TryAddKeyedTransient<ICredentialJsonFileProvider>(name, (sp, name) =>
        {
            return new CredentialJsonFileProvider(sp.GetRequiredKeyedService<IEncryptedJsonFileProvider<Credential>>(name));
        });
        return services;
    }

    /// <summary>
    /// Registers <see cref="ICredentialJsonFileProvider"/> using the default (unnamed) options configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register with.</param>
    /// <param name="configureOptions">The action to configure <see cref="EncryptedJsonFileProviderOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddCredentialJsonFileProvider(
        this IServiceCollection services,
        Action<EncryptedJsonFileProviderOptions> configureOptions)
    {
        services.AddEncryptedJsonFileProvider<Credential>(configureOptions);
        services.TryAddTransient<ICredentialJsonFileProvider, CredentialJsonFileProvider>();
        return services;
    }
}