using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Configuration;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

public interface ICommandAppServiceProviderFactory
{
    IServiceProvider CreateServiceProvider(IServiceCollection services, IConventionContext conventionContext);
}

class DefaultServiceProviderFactory : ICommandAppServiceProviderFactory
{
    private readonly IServiceProvider? _serviceProvider;

    public DefaultServiceProviderFactory()
    {

    }

    public DefaultServiceProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public IServiceProvider CreateServiceProvider(IServiceCollection services, IConventionContext conventionContext)
    {
        services.ApplyConventions(conventionContext);
        new LoggingBuilder(services).ApplyConventions(conventionContext);
        if (_serviceProvider is null)
        return services.BuildServiceProvider();
        return new FallbackServiceProvider(_serviceProvider, services.BuildServiceProvider());
    }
}

class FallbackServiceProvider : IServiceProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceProvider _fallbackServiceProvider;

    public FallbackServiceProvider(IServiceProvider serviceProvider, IServiceProvider fallbackServiceProvider)
    {
        _serviceProvider = serviceProvider;
        _fallbackServiceProvider = fallbackServiceProvider;
    }

    public object? GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType) ?? _fallbackServiceProvider.GetService(serviceType);
    }
}

internal class LoggingBuilder : ILoggingBuilder
{
    public LoggingBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}

class ConventionTypeRegistrar : ITypeRegistrar
{
    private readonly IConventionContext _conventionContext;
    private readonly ServiceCollection _services;

    public ConventionTypeRegistrar(IConventionContext conventionContext)
    {
        _conventionContext = conventionContext;
        _services = new ServiceCollection();
    }

    public void Register(Type service, Type implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        _services.AddSingleton(service, _ => factory());
    }

    public ITypeResolver Build()
    {
        _services.AddSingleton(_conventionContext.Get<IConfiguration>());
        var factory = _conventionContext.GetOrAdd<ICommandAppServiceProviderFactory>(() => new DefaultServiceProviderFactory());
        return new ConventionTypeResolver(factory.CreateServiceProvider(_services, _conventionContext));
    }
}

class ConventionTypeResolver : ITypeResolver
{
    private readonly IServiceProvider _serviceProvider;

    public ConventionTypeResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object? Resolve(Type? type)
    {
        return ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type!);
    }
}

public static class App
{
    public static ICommandApp Create(Action<ConventionContextBuilder>? buildConventions = null)
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>()).UseDependencyContext(DependencyContext.Default);
        buildConventions?.Invoke(builder);
        return Create(builder);
    }

    public static ICommandApp Create(ConventionContextBuilder builder)
    {
        return Create(ConventionContext.From(builder));
    }

    public static ICommandApp Create(IConventionContext context)
    {
        var app = new CommandApp(new ConventionTypeRegistrar(context));
        Configure(app, context);

        return app;
    }

    public static ICommandApp Create(IHostBuilder hostBuilder)
    {
        ConventionContextBuilder builder;
        if (hostBuilder.Properties.TryGetValue(typeof(ConventionContextBuilder), out var o) && o is ConventionContextBuilder b)
        {
            builder = b;
        }
        else
        {
            builder = new ConventionContextBuilder(new Dictionary<object, object?>()).UseDependencyContext(DependencyContext.Default);
            hostBuilder.Properties.Add(typeof(ConventionContextBuilder), builder);
        }

        var app = Create(builder);
        app.Configure(
            configurator =>
            {
                configurator.Settings.Registrar.RegisterInstance(hostBuilder);
                configurator.Settings.Interceptor = new HostingInterceptor(hostBuilder, configurator.Settings.Interceptor);
            }
        );

        return app;
    }

    public static ICommandApp Create<TDefaultCommand>(Action<ConventionContextBuilder>? buildConventions = null) where TDefaultCommand : class, ICommand
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>()).UseDependencyContext(DependencyContext.Default);
        buildConventions?.Invoke(builder);
        return Create<TDefaultCommand>(builder);
    }

    public static ICommandApp Create<TDefaultCommand>(ConventionContextBuilder builder) where TDefaultCommand : class, ICommand
    {
        var context = ConventionContext.From(builder);
        var app = new CommandApp<TDefaultCommand>(new ConventionTypeRegistrar(context));
        Configure(app, context);

        return app;
    }

    private static void Configure(ICommandApp app, IConventionContext context)
    {
        app.Configure(
            configurator =>
            {
                configurator.Settings.Registrar.RegisterInstance(context);
                var configuration = ApplyConfiguration(context);
                context.Set<IConfiguration>(configuration);

                foreach (var item in context.Conventions.Get<ICommandLineConvention, CommandLineConvention>())
                {
                    if (item is ICommandLineConvention convention)
                    {
                        convention.Register(context, configurator);
                    }
                    else if (item is CommandLineConvention @delegate)
                    {
                        @delegate(context, configurator);
                    }
                }
            }
        );
    }

    private static IConfigurationRoot ApplyConfiguration(IConventionContext conventionContext)
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.UseLocalConfiguration(conventionContext.GetOrAdd(() => new ConfigOptions()));

        // Insert after all the normal configuration but before the environment specific configuration
        IConfigurationSource? source = null;
        foreach (var item in configurationBuilder.Sources.Reverse())
        {
            if (item is CommandLineConfigurationSource ||
                ( item is EnvironmentVariablesConfigurationSource env && ( string.IsNullOrWhiteSpace(env.Prefix) ||
                                                                           string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) ) ||
                ( item is JsonConfigurationSource a && string.Equals(
                    a.Path,
                    "secrets.json",
                    StringComparison.OrdinalIgnoreCase
                ) ))
            {
                continue;
            }

            source = item;
            break;
        }

        var index = source == null
            ? configurationBuilder.Sources.Count - 1
            : configurationBuilder.Sources.IndexOf(source);

        var cb = new ConfigurationBuilder().ApplyConventions(conventionContext, configurationBuilder.Build());

        configurationBuilder.Sources.Insert(
            index + 1,
            new ChainedConfigurationSource
            {
                Configuration = cb.Build(),
                ShouldDisposeConfiguration = true
            }
        );

        return configurationBuilder.Build();
    }
}

/// <summary>
///     Delegate CommandLineConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="app"></param>
public delegate void CommandLineConvention(IConventionContext context, IConfigurator app);

/// <summary>
///     ICommandLineConvention
///     Implements the <see cref="IConvention" />
/// </summary>
/// <seealso cref="IConvention" />
public interface ICommandLineConvention : IConvention
{
    /// <summary>
    ///     Register additional services with the service collection
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="app"></param>
    void Register(IConventionContext context, IConfigurator app);
}

public class AppSettings : CommandSettings
{
    /// <summary>
    ///     Gets the log.
    /// </summary>
    /// <value>The log.</value>
    [CommandOption("-l|--log")]
    [UsedImplicitly]
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="AppSettings" /> is
    ///     verbose.
    /// </summary>
    /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
    [CommandOption("-v|--verbose")]
    [Description("Verbose logging")]
    [UsedImplicitly]
    public bool Verbose
    {
        get => LogLevel == global::Microsoft.Extensions.Logging.LogLevel.Debug;
        set => LogLevel = value ? global::Microsoft.Extensions.Logging.LogLevel.Debug : LogLevel;
    }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="AppSettings" /> is trace.
    /// </summary>
    /// <value><c>true</c> if trace; otherwise, <c>false</c>.</value>
    [CommandOption("-t|--trace")]
    [Description("Trace logging")]
    [UsedImplicitly]
    public bool Trace
    {
        get => LogLevel == global::Microsoft.Extensions.Logging.LogLevel.Trace;
        set => LogLevel = value ? global::Microsoft.Extensions.Logging.LogLevel.Trace : LogLevel;
    }
}

class HostingInterceptor : ICommandInterceptor
{
    private readonly IHostBuilder _hostBuilder;
    private readonly ICommandInterceptor? _commandInterceptor;

    public HostingInterceptor(IHostBuilder hostBuilder, ICommandInterceptor? commandInterceptor)
    {
        _hostBuilder = hostBuilder;
        _commandInterceptor = commandInterceptor;
    }

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        _commandInterceptor?.Intercept(context, settings);
        if (settings is not AppSettings appSettings) appSettings = new AppSettings();
        var result = new HostingResult();
        CommandLineArgumentsExtractorCommand.PopulateResult(result, context, appSettings);
        _hostBuilder.Properties.Add(typeof(HostingResult), result);
    }
}

class HostingCommand : AsyncCommand<AppSettings>
{
    private readonly IHostBuilder _hostBuilder;

    public HostingCommand(IHostBuilder hostBuilder)
    {
        _hostBuilder = hostBuilder;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, AppSettings settings)
    {
        var result = new HostingResult();
        _hostBuilder.Properties.Add(typeof(HostingResult), result);
        CommandLineArgumentsExtractorCommand.PopulateResult(result, context, settings);
        await _hostBuilder.Build().RunAsync();
        return 0;
    }
}

class HostingResult
{
    public IDictionary<string, string>? Configuration { get; set; }
    public IRemainingArguments? Arguments { get; set; }
}

class CommandLineArgumentsExtractorCommand : Command<AppSettings>
{
    private readonly HostingResult _result;

    public CommandLineArgumentsExtractorCommand(HostingResult result) => _result = result;

    public override int Execute(CommandContext context, AppSettings settings)
    {
        PopulateResult(_result, context, settings);
        return 0;
    }

    public static void PopulateResult(HostingResult result, CommandContext context, AppSettings settings)
    {
        result.Configuration = new Dictionary<string, string>
        {
            [nameof(AppSettings.Trace)] = settings.Trace.ToString(CultureInfo.InvariantCulture),
            [nameof(AppSettings.Verbose)] = settings.Verbose.ToString(CultureInfo.InvariantCulture),
            [nameof(AppSettings.LogLevel)] = settings.LogLevel.HasValue ? settings.LogLevel.Value.ToString() : ""
        }.ToDictionary(z => $"{nameof(AppSettings)}:{z.Key}", z => z.Value);
        result.Arguments = context.Remaining;
    }
}
