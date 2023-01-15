using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

[ExportConvention]
public class ConsoleConvention : IServiceConvention, IHostingConvention
{
    private bool _isHostBuilder;

    public void Register(IConventionContext context, IHostBuilder builder)
    {
        _isHostBuilder = true;
        builder.ConfigureAppConfiguration(
            (builderContext, configurationBuilder) =>
            {
                var sourcesToRemove = configurationBuilder.Sources.OfType<CommandLineConfigurationSource>().ToList();
                var appSettings = new AppSettingsConfigurationSource(sourcesToRemove.FirstOrDefault()?.Args ?? Array.Empty<string>());
                configurationBuilder.Add(appSettings);
                context.Set(appSettings);
            }
        );
    }

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        // We just bail out, the environment is not correct!
        if (!_isHostBuilder)
        {
            return;
        }

        var registry = new ConventionTypeRegistrar();
        var command = new CommandApp(registry);
        var consoleResult = new ConsoleResult();

        command.Configure(
            configurator =>
            {
                var interceptor = new ConsoleInterceptor(
                    context.Get<AppSettingsConfigurationSource>()!,
                    consoleResult,
                    configurator.Settings.Interceptor
                );
                configurator.SetInterceptor(interceptor);

                foreach (var item in context.Conventions.Get<ICommandLineConvention, CommandLineConvention>())
                {
                    if (!context.Properties.TryGetValue(typeof(ConsoleConvention), out _))
                    {
                        context.Properties.Add(typeof(ConsoleConvention), true);
                    }

                    if (item is ICommandLineConvention convention)
                    {
                        convention.Register(context, configurator);
                    }
                    else if (item is CommandLineConvention @delegate)
                    {
                        @delegate(context, configurator);
                    }
                }

                var defaultCommandProperty = configurator.GetType().GetProperty("DefaultCommand");
                if (defaultCommandProperty?.GetValue(configurator) == null)
                {
                    command.SetDefaultCommand<DefaultCommand>();
                }
            }
        );

        // We don't want to run if there were no possible command conventions.
        if (!context.Properties.TryGetValue(typeof(ConsoleConvention), out _))
        {
            return;
        }

        services.AddSingleton<IAnsiConsole>(_ => (IAnsiConsole)registry.GetService(typeof(IAnsiConsole))!);
        services.AddSingleton<IRemainingArguments>(_ => (IRemainingArguments)registry.GetService(typeof(IRemainingArguments))!);
        services.AddSingleton(consoleResult);
        services.AddHostedService<ConsoleWorker>();
        services.AddSingleton(context.Get<AppSettingsConfigurationSource>()!);
        services.AddSingleton<ICommandApp>(
            provider =>
            {
                registry.SetServiceProvider(provider);
                return command;
            }
        );
    }
}
