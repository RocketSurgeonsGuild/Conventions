using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rocket.Surgery.CommandLine;

/// <summary>
///     Convention for console applications
/// </summary>
[ExportConvention]
public class ConsoleConvention : IHostApplicationAsyncConvention
{
    /// <inheritdoc />
    public async ValueTask Register(IConventionContext context, IHostApplicationBuilder builder, CancellationToken cancellationToken)
    {
        var sourcesToRemove = builder.Configuration.Sources.OfType<CommandLineConfigurationSource>().ToList();
        var appSettings = new AppSettingsConfigurationSource(sourcesToRemove.FirstOrDefault()?.Args ?? Array.Empty<string>());
        builder.Configuration.Add(appSettings);
        context.Set(appSettings);

        var registry = new ConventionTypeRegistrar();
        var command = new CommandApp(registry);
        var consoleResult = new ConsoleResult();
        var found = false;

        command.Configure(
            configurator =>
            {
                var interceptor = new ConsoleInterceptor(
                    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
                    context.Get<AppSettingsConfigurationSource>()!
                );
                configurator.SetInterceptor(interceptor);
            }
        );

        foreach (var item in context.Conventions.GetAll())
        {
            switch (item)
            {
                case ICommandAppConvention convention:
                    convention.Register(context, command);
                    found = true;
                    break;
                case CommandAppConvention @delegate:
                    @delegate(context, command);
                    found = true;
                    break;
                case ICommandAppAsyncConvention convention:
                    await convention.Register(context, command, cancellationToken);
                    found = true;
                    break;
                case CommandAppAsyncConvention @delegate:
                    await @delegate(context, command, cancellationToken);
                    found = true;
                    break;
                case ICommandLineConvention convention:
                    command.Configure(configurator => convention.Register(context, configurator));
                    found = true;
                    break;
                case CommandLineConvention @delegate:
                    command.Configure(configurator => @delegate(context, configurator));
                    found = true;
                    break;
                case ICommandLineAsyncConvention convention:
                    {
                        var itcs = new TaskCompletionSource();
                        cancellationToken.Register(() => itcs.TrySetCanceled());
                        // ReSharper disable once AsyncVoidLambda
                        command.Configure(
                            async configurator =>
                            {
                                try
                                {
                                    await convention.Register(context, configurator, cancellationToken);
                                    itcs.SetResult();
                                }
                                catch (Exception e)
                                {
                                    itcs.SetException(e);
                                }
                            }
                        );
                        await itcs.Task;
                    }
                    found = true;
                    break;
                case CommandLineAsyncConvention @delegate:
                    {
                        var dtcs = new TaskCompletionSource();
                        cancellationToken.Register(() => dtcs.TrySetCanceled());
                        // ReSharper disable once AsyncVoidLambda
                        command.Configure(
                            async configurator =>
                            {
                                try
                                {
                                    await @delegate(context, configurator, cancellationToken);
                                    dtcs.SetResult();
                                }
                                catch (Exception e)
                                {
                                    dtcs.SetException(e);
                                }
                            }
                        );
                        found = true;
                        await dtcs.Task;
                    }
                    break;
            }
        }

        command.Configure(
            configurator =>
            {
                var defaultCommandProperty = configurator.GetType().GetProperty("DefaultCommand");
                if (defaultCommandProperty?.GetValue(configurator) == null) command.SetDefaultCommand<DefaultCommand>();
            }
        );

        // We don't want to run if there were no possible command conventions.
        if (!found) return;

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
