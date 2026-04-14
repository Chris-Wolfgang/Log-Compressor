using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Framework;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Wolfgang.LogCompressor
{

    [Command
    (
        Name = "logc",

        Description = "Cross-platform CLI tool to compress log files",

        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw,

        ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated
    )]
    [Subcommand(typeof(Compress))]
    [Subcommand(typeof(Bundle))]
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            try
            {
                return await new HostBuilder()
                    .AddConfigurationFile
                    (
                        ConfigurationFileMethod.SingleFile,
                        optional: false,
                        reloadOnChange: false
                    )

                    .UseSerilog((context, configuration) =>
                    {
                        configuration
                            .ReadFrom.Configuration(context.Configuration)
                            .Enrich.WithProperty("Version", Assembly.GetEntryAssembly()?.GetName().Version)
                            ;
                    })

                    .ConfigureServices((_, serviceCollection) =>
                    {
                        serviceCollection
                            .AddSingleton<IReporter, ConsoleReporter>()
                            .AddSingleton<IFileSystem, FileSystemWrapper>()
                            .AddSingleton<IFileFilter, FileFilterService>()
                            .AddSingleton<IFileNamer, FileNamingService>()
                            .AddSingleton<CompressionStrategyFactory>()
                            .AddTransient<CompressService>()
                            .AddTransient<BundleService>()
                            ;
                    })
                    .RunCommandLineApplicationAsync<Program>(args);
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
        internal int OnExecute
        (
            CommandLineApplication<Program> application
        )
        {
            application.ShowHelp();
            return ExitCode.Success;
        }
    }
}
