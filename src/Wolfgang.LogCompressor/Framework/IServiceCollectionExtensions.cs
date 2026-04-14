using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wolfgang.LogCompressor.Framework;

// ReSharper disable once InconsistentNaming
[ExcludeFromCodeCoverage]
internal static class IServiceCollectionExtensions
{
#pragma warning disable SYSLIB1104 // Binding logic not generated for generic Get<T> — acceptable for config binding
    internal static IServiceCollection BindConfigSection<T>
    (
        this IServiceCollection services,
        string path
    ) where T : class, new()
    {
        services.AddSingleton
        (
            provider => provider
                    .GetRequiredService<IConfiguration>()
                    .GetSection(path)
                    .Get<T>()

                ?? throw new ConfigurationErrorsException
                (
                    $"Could not bind to the config section '{path}'. " +
                    "Make sure the section exists in the config file and matches " +
                    "the specified class."
                )
        );

        return services;
    }
#pragma warning restore SYSLIB1104
}
