using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
/// Convention for console applications
/// </summary>
[ExportConvention]
public class ConsoleConvention : IHostApplicationConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IHostApplicationBuilder builder)
    {
        var sourcesToRemove = builder.Configuration.Sources.OfType<CommandLineConfigurationSource>().ToList();
        var appSettings = new AppSettingsConfigurationSource(sourcesToRemove.FirstOrDefault()?.Args ?? Array.Empty<string>());
        builder.Configuration.Add(appSettings);
        context.Set(appSettings);


        var registry = new ConventionTypeRegistrar();
        var command = new CommandApp(registry);
        var consoleResult = new ConsoleResult();

        command.Configure(
            configurator =>
            {
                var interceptor = new ConsoleInterceptor(
                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                    context.Get<AppSettingsConfigurationSource>()!,
                    configurator.Settings.Interceptor
                );
                configurator.SetInterceptor(interceptor);

                foreach (var item in context.Conventions.Get<ICommandLineConvention, CommandLineConvention>())
                {
                    if (!context.Properties.TryGetValue(typeof(ConsoleConvention), out _))
                    {
                        context.Properties.Add(typeof(ConsoleConvention), true);
                    }

                    switch (item)
                    {
                        case ICommandLineConvention convention:
                            convention.Register(context, configurator);
                            break;
                        case CommandLineConvention @delegate:
                            @delegate(context, configurator);
                            break;
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

        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        builder.Services.AddSingleton<IAnsiConsole>(_ => (IAnsiConsole)registry.GetService(typeof(IAnsiConsole))!);
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        builder.Services.AddSingleton<IRemainingArguments>(_ => (IRemainingArguments)registry.GetService(typeof(IRemainingArguments))!);
        builder.Services.AddSingleton(consoleResult);
        builder.Services.AddHostedService<ConsoleWorker>();
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        builder.Services.AddSingleton(context.Get<AppSettingsConfigurationSource>()!);
        builder.Services.AddSingleton<ICommandApp>(
            provider =>
            {
                registry.SetServiceProvider(provider);
                return command;
            }
        );
    }
}
