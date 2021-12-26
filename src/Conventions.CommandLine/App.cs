using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Configuration;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

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
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseDependencyContext(DependencyContext.Default);
        buildConventions?.Invoke(builder);
        return Create<TDefaultCommand>(builder);
    }

    public static ICommandApp Create<TDefaultCommand>(Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions)
        where TDefaultCommand : class, ICommand
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseDependencyContext(DependencyContext.Default)
                     .WithConventionsFrom(getConventions);
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
            var itemType = item.GetType();
            if (itemType is { Name: "CommandLineConfigurationSource" } ||
                itemType is { Name: "EnvironmentVariablesConfigurationSource" } ||
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
