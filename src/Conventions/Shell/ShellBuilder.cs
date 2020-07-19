using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using LoxSmoke.DocXml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Shell
{
    /// <summary>
    /// Logging Builder
    /// Implements the <see cref="IShellConventionContext" />
    /// </summary>
    /// <seealso cref="IShellConventionContext" />
    public class ShellBuilder :
        ConventionBuilder<ShellBuilder, IShellConvention, ShellConventionDelegate>,
        IShellConventionContext
    {
        private readonly RootCommand _command;
        private Lazy<DocXmlReader> _docXmlReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineBuilder" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">diagnosticSource</exception>
        public ShellBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        ) : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            _command = new RootCommand();
            Builder = new CommandLineBuilder(_command)
               .UseDefaults()
               .UseAnsiTerminalWhenAvailable()
               .EnableDirectives()
               .EnablePosixBundling();
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
            _docXmlReader = new Lazy<DocXmlReader>(() => new DocXmlReader(assemblyProvider.GetAssemblies()));
        }

        /// <summary>
        /// Builds the specified entry assembly.
        /// </summary>
        /// <returns>ICommandLine.</returns>
        public ParseResult Parse(string[] args)
        {
            var verbose = new Option<bool>(new[] { "-v", "--verbose" }, "Enable verbose logging");
            var trace = new Option<bool>(new[] { "-t", "--trace" }, "Enable trace logging");
            var debug = new Option<bool>(new[] { "-d", "--debug" }, "Enable debug logging");
            var logLevel = new Option<LogLevel?>(new[] { "-ll", "--log-level" }, "Set the log level");

            _command.AddGlobalOption(verbose);
            _command.AddGlobalOption(trace);
            _command.AddGlobalOption(debug);
            _command.AddGlobalOption(logLevel);

            Composer.Register(Scanner, this, typeof(IShellConvention), typeof(ShellConventionDelegate));

            var parser = Builder.Build();

            return parser.Parse(args);
        }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        /// The commandline builder
        /// </summary>
        public CommandLineBuilder Builder { get; }

        /// <summary>
        /// Uses the specific console
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        public IShellConventionContext UseConsole(IConsole console)
        {
            Console = console;
            return this;
        }

        /// <inheritdoc />
        public IShellConventionContext AddCommand<T>(string? description = null)
        {
            AddCommand(typeof(T), alias, description);
            return this;
        }

        public IShellConventionContext AddCommand(Type commandType, string? description = null)
        {
            var typeComments = new Lazy<TypeComments>(() => _docXmlReader.Value.GetTypeComments(commandType));
            var aliases = _docXmlReader.Value.GetTypeComments(commandType).Example?.Split(',');
            if (aliases == null || aliases.Length == 0)
            {
                var inferAlias = commandType.Name.ToLowerInvariant();
                if (inferAlias.EndsWith("command", StringComparison.OrdinalIgnoreCase))
                    inferAlias = inferAlias.Substring(0, inferAlias.Length - inferAlias.IndexOf("command", StringComparison.OrdinalIgnoreCase));
                if (inferAlias.EndsWith("shell", StringComparison.OrdinalIgnoreCase))
                    inferAlias = inferAlias.Substring(0, inferAlias.Length - inferAlias.IndexOf("shell", StringComparison.OrdinalIgnoreCase));
                aliases = new[] { inferAlias };
            }

            description ??= _docXmlReader.Value.GetTypeComments(commandType).Summary;

            var command = new Command(aliases.First(), description);
            foreach (var alias in aliases.Skip(1)) command.AddAlias(alias);

            return this;
        }

        /// <summary>
        /// The console
        /// </summary>
        public IConsole? Console { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ShellCommand : Attribute
    {
        public IEnumerable<string> Aliases { get; }

        // See the attribute guidelines at
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public ShellCommand(params string[] aliases)
        {
            Aliases = aliases;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ChildShellCommand : Attribute
    {
        public Type CommandType { get; }

        // See the attribute guidelines at
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        public ChildShellCommand(Type commandType)
        {
            CommandType = commandType;
        }
    }

    /// <summary>
    /// ApplicationStateExtensions.
    /// </summary>
    public static class ShellBuilderExtensions
    {
        /// <summary>
        /// Adds the state of the application.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="parseResult">The parse result.</param>
        /// <returns>IConfigurationBuilder.</returns>
        [NotNull]
        public static IConfigurationBuilder AddShellState(this IConfigurationBuilder builder, [NotNull] ParseResult parseResult)
        {
            if (parseResult == null)
            {
                throw new ArgumentNullException(nameof(parseResult));
            }

            var debug = parseResult.ValueForOption<bool>("--debug");
            var trace = parseResult.ValueForOption<bool>("--trace");
            var verbose = parseResult.ValueForOption<bool>("--verbose");
            var loglevel = parseResult.ValueForOption<LogLevel?>("--log-level");
            builder.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    [nameof(debug)] = debug.ToString(CultureInfo.InvariantCulture),
                    [nameof(trace)] = trace.ToString(CultureInfo.InvariantCulture),
                    [nameof(verbose)] = verbose.ToString(CultureInfo.InvariantCulture)
                }
            );

            if (loglevel.HasValue)
            {
                builder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        [nameof(loglevel)] = loglevel.ToString()
                    }
                );
            }

            return builder;
        }
    }
}