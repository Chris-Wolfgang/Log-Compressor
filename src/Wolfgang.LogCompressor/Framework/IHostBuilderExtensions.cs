using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Wolfgang.LogCompressor.Framework;

#pragma warning disable MA0048 // Enum co-located with its only consumer
internal enum ConfigurationFileMethod
#pragma warning restore MA0048
{
    SingleFile,
    OneFilePerEnvironment
}



[ExcludeFromCodeCoverage]
internal static class IHostBuilderExtensions
{
    /// <summary>
    /// Adds a configuration file to the host builder.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="method">The configuration file strategy to use.</param>
    /// <param name="optional">Whether the configuration file is optional.</param>
    /// <param name="reloadOnChange">Whether to reload the configuration when the file changes.</param>
    /// <returns>The configured <see cref="IHostBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="method"/> is not a valid value.</exception>
    public static IHostBuilder AddConfigurationFile
    (
        this IHostBuilder builder,
        ConfigurationFileMethod method,
        bool optional = false,
        bool reloadOnChange = false
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        return method switch
        {
            ConfigurationFileMethod.SingleFile => AddSingleConfigFile(builder, optional, reloadOnChange),
            ConfigurationFileMethod.OneFilePerEnvironment => AddConfigFileForEnvironment(builder, optional, reloadOnChange),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(method), method, message: null)
        };
    }



    private static IHostBuilder AddSingleConfigFile
    (
        this IHostBuilder builder,
        bool optional,
        bool reloadOnChange
    )
    {
        builder
            .ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("AppSettings.json", optional, reloadOnChange)
                    .AddEnvironmentVariables();
            });

        return builder;
    }



    private static IHostBuilder AddConfigFileForEnvironment
    (
        this IHostBuilder builder,
        bool optional,
        bool reloadOnChange
    )
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (string.IsNullOrWhiteSpace(environment))
        {
            Environment.FailFast("System variable DOTNET_ENVIRONMENT is not set.");
        }

        builder
            .ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile($"AppSettings.{environment}.json", optional, reloadOnChange)
                    .AddEnvironmentVariables();
            });
        return builder;
    }
}
