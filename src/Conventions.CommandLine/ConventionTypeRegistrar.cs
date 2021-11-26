using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
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
    public IServiceProvider CreateServiceProvider(IServiceCollection services, IConventionContext conventionContext)
    {
        services.ApplyConventions(conventionContext);
        new LoggingBuilder(services).ApplyConventions(conventionContext);
        return services.BuildServiceProvider();
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

    private  static void Configure(ICommandApp app, IConventionContext context)
    {
        app.Configure(
            configurator =>
            {
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
    [CommandOption("--log")]
    [UsedImplicitly]
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    ///     Gets a value indicating whether this <see cref="AppSettings" /> is
    ///     verbose.
    /// </summary>
    /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
    [CommandOption("-v|--verbose")]
    [Description("Verbose logging")]
    [UsedImplicitly]
    public bool Verbose {
        get => LogLevel == LogLevel.Debug;
        set => LogLevel = value ? LogLevel.Debug : LogLevel;
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
        get => LogLevel == LogLevel.Trace;
        set => LogLevel = value ? LogLevel.Trace : LogLevel;
    }
}


/// <summary>
///     ApplicationStateExtensions.
/// </summary>
public static class AppSettingsExtensions
{
    /// <summary>
    ///     Adds the state of the application.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="state">The state.</param>
    /// <returns>IConfigurationBuilder.</returns>
    public static IConfigurationBuilder AddAppSettings(
        this IConfigurationBuilder builder,
        AppSettings state
    )
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        builder.AddInMemoryCollection(
            new Dictionary<string, string>
            {
                [nameof(AppSettings.Trace)] = state.Trace.ToString(CultureInfo.InvariantCulture),
                [nameof(AppSettings.Verbose)] = state.Verbose.ToString(CultureInfo.InvariantCulture),
                [nameof(AppSettings.LogLevel)] = state.LogLevel.ToString()
            }.ToDictionary(z => $"{nameof(AppSettings)}:{z.Key}", z => z.Value)
        );

        return builder;
    }
}
