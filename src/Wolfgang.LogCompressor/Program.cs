using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Framework;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor;

[Command
(
    Name = "logc",

    Description = "Cross-platform CLI tool to compress log files",

    UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw,

    ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated
)]
[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
[Subcommand(typeof(Compress))]
[Subcommand(typeof(Bundle))]
[Subcommand(typeof(Decompress))]
[Subcommand(typeof(Init))]
[ExcludeFromCodeCoverage]
internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            // McMaster turns blank and indented-comment lines in an @response file into empty
            // arguments; hand it a cleaned copy where needed (see ResponseFileSanitizer).
            using var sanitized = await ResponseFileSanitizer.SanitizeAsync(args).ConfigureAwait(false);

            return await new HostBuilder()
                .AddConfigurationFile
                (
                    ConfigurationFileMethod.SingleFile,
                    optional: false,
                    reloadOnChange: false
                )

                .UseSerilog((context, configuration) =>
                {
                    var options = context.Configuration
                        .GetSection("Logging")
                        .Get<LoggingOptions>() ?? new LoggingOptions();

                    configuration
                        .Apply(options)
                        .Enrich.WithProperty("Version", Assembly.GetEntryAssembly()?.GetName().Version)
                        ;
                })

                .ConfigureServices((_, serviceCollection) =>
                {
                    serviceCollection
                        .AddSingleton(TimeProvider.System)
                        .AddSingleton<IReporter, ConsoleReporter>()
                        .AddSingleton<IFileSystem, FileSystemWrapper>()
                        .AddSingleton<IFileFilter, FileFilterService>()
                        .AddSingleton<IFileNamer, FileNamingService>()
                        .AddSingleton<IArchiveVerifier, ArchiveVerifier>()
                        .AddSingleton<CompressionStrategyFactory>()
                        .AddSingleton<ReportService>()
                        .AddSingleton<RetentionService>()
                        .AddTransient<CompressService>()
                        .AddTransient<BundleService>()
                        .AddTransient<DecompressService>()
                        ;
                })
                .RunCommandLineApplicationAsync<Program>
                (
                    sanitized.Args,
                    PrecompileValueParsers
                );
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.Message);
            Log.Logger.Fatal(e, "Unhandled exception: {Message}", e.Message);
            return ExitCode.UnhandledException;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }



    /// <summary>
    /// This method is called if the user does not specify a sub command.
    /// </summary>
    /// <param name="application">The command line application.</param>
    /// <returns>0 on success or any positive number for failure.</returns>
#pragma warning disable S2325 // Instance method by McMaster convention: CommandLineUtils resolves OnExecute via instance reflection — a static method is not discovered (verified: 'No method named OnExecute could be found').
    internal int OnExecute
    (
        CommandLineApplication<Program> application
    )
    {
        application.ShowHelp();
        return ExitCode.Success;
    }
#pragma warning restore S2325



    /// <summary>
    /// Makes the value parsers McMaster needs for this app's option types exist in the Native AOT
    /// binary.
    /// </summary>
    /// <remarks>
    /// McMaster.Extensions.CommandLineUtils picks a parser for each option by calling
    /// <c>ValueParserProvider.GetParser&lt;T&gt;</c> through <c>MethodInfo.MakeGenericMethod</c>.
    /// Native AOT cannot create code at run time, so for a value type <c>T</c> that instantiation
    /// only exists if something calls it statically; without it every command that has an
    /// <see cref="int"/>? option fails at startup with "missing native code". Calling
    /// <c>GetParser&lt;T&gt;</c> here makes the compiler emit that instantiation. Only the
    /// value-typed options that take a value need one: today that is <see cref="int"/>?
    /// (<c>--older-than</c>, <c>--delete-archives-older-than</c>); <see cref="bool"/> switches take
    /// no value, and reference types (<see cref="string"/>, <see cref="string"/>[]) share one
    /// instantiation. A new option of another value type (an enum, <see cref="long"/>,
    /// <see cref="TimeSpan"/>) must be added here; scripts/smoke-published.ps1 fails until it is.
    /// </remarks>
    /// <param name="application">The root command line application.</param>
    private static void PrecompileValueParsers
    (
        CommandLineApplication<Program> application
    )
    {
        _ = application.ValueParsers.GetParser<int?>();
    }



    private static string GetVersion()
    {
        return typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
    }
}
