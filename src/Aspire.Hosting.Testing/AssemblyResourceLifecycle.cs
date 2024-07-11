using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public class AssemblyResourceLifecycle(
    DistributedApplicationExecutionContext executionContext,
    ResourceLoggerService loggerService
    ) : IDistributedApplicationLifecycleHook
{
    public async Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var assemblyResource in appModel.Resources.OfType<AssemblyResource>())
        {
            await StartAssemblyResource(appModel, assemblyResource, cancellationToken);
        }
    }

    private async Task<IProgramFixture> StartAssemblyResource(
        DistributedApplicationModel appModel,
        AssemblyResource assemblyResource,
        CancellationToken cancellationToken
    )
    {
        var args = new List<string>();
        var urls = new List<string>();
        var environmentVariables = new Dictionary<string, string>();

        if (assemblyResource.TryGetEndpoints(out var endpoints))
        {
            urls.AddRange(
                endpoints
                   .Where(e => e.Port.HasValue)
                   .OrderBy(z => z.UriScheme == "https")
                   .Select(e => $"{e.UriScheme}://localhost:{e.Port}")
            );
            args.Add($"--urls={string.Join(";", urls)}");
        }

        if (assemblyResource.TryGetAnnotationsOfType<CommandLineArgsCallbackAnnotation>(out var commandLineArgsCallbackAnnotations))
        {
            var context = new CommandLineArgsCallbackContext(new List<object>(), cancellationToken);
            foreach (var annotation in commandLineArgsCallbackAnnotations)
            {
                await annotation.Callback(context);
            }

            foreach (var arg in context.Args)
            {
                var value = arg switch
                            {
                                string s                     => s,
                                IValueProvider valueProvider => await GetValue(valueProvider, cancellationToken).ConfigureAwait(false),
                                null                         => null,
                                _                            => throw new InvalidOperationException($"Unexpected value for arg \"{arg}\"."),
                            };

                if (value is { }) args.Add(value);
            }
        }

        if (assemblyResource.TryGetEnvironmentVariables(out var environmentCallbackAnnotations))
        {
            var context = new EnvironmentCallbackContext(executionContext, new());
            foreach (var annotation in environmentCallbackAnnotations)
            {
                await annotation.Callback(context);
            }

            foreach (var kvp in context.EnvironmentVariables)
            {
                if (kvp.Key == "ASPNETCORE_URLS") continue;
                if (kvp.Key == "OTEL_RESOURCE_ATTRIBUTES")
                {
                    environmentVariables.Add(kvp.Key, $"service.instance.id={Guid.NewGuid():N}");
                    continue;
                }

                if (kvp.Key == "OTEL_SERVICE_NAME")
                {
                    environmentVariables.Add(kvp.Key, assemblyResource.Name);
                    continue;
                }

                var value = kvp.Value switch
                            {
                                string s => s,
                                IValueProvider valueProvider => await GetValue(valueProvider, cancellationToken).ConfigureAwait(false),
                                null => null,
                                _ => throw new InvalidOperationException($"Unexpected value for environment variable \"{kvp.Key}\"."),
                            };

                if (value is { }) environmentVariables.Add(kvp.Key, value);
            }
        }

        var loggerFactory = new LoggerFactory();
        var logger = loggerService.GetLogger(assemblyResource);
        loggerFactory.AddProvider(new AspireLoggerProvider(logger));

        var hostTcs = new TaskCompletionSource<IHost>();

        try
        {
            // This will wait until the semaphore is released in many tests are happening in parallel
            using var disposable = ImportHelpers.SetExternalConfigureMethodWithLock(
                (builder, ct) =>
                {
                    builder.Set(HostType.UnitTest);
                    builder.PrependConvention(
                        new ProgramInstanceConvention(
                            assemblyResource.Project.GetProjectMetadata(),
                            loggerFactory,
                            environmentVariables.ToFrozenDictionary()
                        )
                    );
                    builder.Set<ILoggerFactory>(loggerFactory);
                    builder.ConfigureServices(s => s.AddHostedService<HostedStartedService>(provider => new(provider.GetRequiredService<IHost>(), hostTcs)));

                    foreach (var ext in assemblyResource.Extensions)
                    {
                        builder.AppendConvention(ext);
                    }

                    return assemblyResource.Configure(builder, ct);
                },
                cancellationToken
            );

            Task runAction()
            {
                return assemblyResource.EntryPoint.Invoke(null, [args.ToArray()]) switch
                       {
                           Task<int> t      => t,
                           Task t           => t,
                           ValueTask<int> t => t.AsTask(),
                           ValueTask t      => t.AsTask(),
                           { } v            => throw new NotSupportedException($"The following return type is not supported {v.GetType().FullName}"),
                           _                => throw new NotSupportedException("Null returned :("),
                       };
            }

            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        await runAction();
                    }
                    catch (TargetInvocationException ex)
                    {
                        hostTcs.TrySetException(ex.InnerException ?? ex);
                    }
                    catch (Exception ex)
                    {
                        hostTcs.TrySetException(ex);
                    }
                },
                cancellationToken
            );
        }
        catch (OperationCanceledException)
        {
            throw new InvalidOperationException(
                $"Timed out waiting for the entry point to build the IHost after {SetupDefaultTimeout()}. This timeout can be modified using the 'ASPIRE_DEFAULT_TIMEOUT_IN_SECONDS' environment variable."
            );
        }
        catch (AggregateException) when (hostTcs.Task.IsCompleted) { }

        var host = await hostTcs.Task;
        var program = new ProgramInstance(host, loggerFactory, urls, [..assemblyResource.Extensions]);
        foreach (var ext in assemblyResource.Extensions)
        {
            await ext.Start(program, host);
        }

        return program;
    }

    private static async Task<string?> GetValue(IValueProvider valueProvider, CancellationToken cancellationToken)
    {
        return await valueProvider.GetValueAsync(cancellationToken).ConfigureAwait(false);
    }

    private static TimeSpan SetupDefaultTimeout()
    {
        return Debugger.IsAttached
            ? Timeout.InfiniteTimeSpan
            : uint.TryParse(Environment.GetEnvironmentVariable("ASPIRE_DEFAULT_TIMEOUT_IN_SECONDS"), out var result)
                ? TimeSpan.FromSeconds((int)result)
                : TimeSpan.FromMinutes(5);
    }
}

file class ProgramInstance : IProgramFixture, IAsyncDisposable
{
    private readonly ImmutableArray<ITestConvention> _extensions;

    private readonly ILoggerFactory _loggerFactory;
    private readonly IEnumerable<string> _urls;
    private readonly ILookup<Type, ITestConvention> _lookup;

    public ProgramInstance(
        IHost host,
        ILoggerFactory loggerFactory,
        IEnumerable<string> urls,
        ImmutableArray<ITestConvention> extensions
    )
    {
        _loggerFactory = loggerFactory;
        _urls = urls;
        Host = host;
        _extensions = extensions;
        _lookup = extensions.ToLookup(z => z.GetType());
    }

    public T GetExtension<T>() where T : ITestConvention
    {
        return _lookup[typeof(T)].OfType<T>().First();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var ext in _extensions)
        {
            await ext.Stop(this, Host);
        }

        await Host.StopAsync();

        var asyncDisposables = _extensions.OfType<IAsyncDisposable>().ToArray();
        var disposables = _extensions.OfType<IDisposable>().Where(z => z is not IAsyncDisposable);

        foreach (var asyncDisposable in asyncDisposables)
        {
            await asyncDisposable.DisposeAsync();
        }

        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }

        _loggerFactory.Dispose();
        if (Host is IAsyncDisposable h) await h.DisposeAsync();
        else Host.Dispose();
    }

    public IHost Host { get; }

    public Uri BaseAddress => new(_urls.First());
    public IServiceProvider Services => Host.Services;

    public async Task Reset()
    {
        foreach (var ext in _extensions.OfType<IResettableTestConvention>())
        {
            await ext.Reset(this, Host);
        }
    }

    /// <summary>
    ///     Creates an instance of <see cref="HttpClient" /> that automatically follows
    ///     redirects and handles cookies.
    /// </summary>
    /// <returns>The <see cref="HttpClient" />.</returns>
    public HttpClient CreateClient()
    {
        return Host.Services.GetRequiredService<IHttpClientFactory>().CreateClient("Internal");
    }

    /// <summary>
    ///     Set the logger factory when initializing the test
    /// </summary>
    /// <param name="loggerFactory"></param>
    public void SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        ( _loggerFactory as AppFixtureLoggerFactory )?.SetLoggerFactory(loggerFactory);
    }
}

file class AppFixtureLoggerFactory : ILoggerFactory
{
    private readonly List<DeferredLogger> _deferredLoggers = new();
    private ILoggerFactory? _innerLoggerFactory;

    public void SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        _innerLoggerFactory = loggerFactory;
        foreach (var logger in _deferredLoggers)
        {
            logger.SetLogger(loggerFactory.CreateLogger(logger.CategoryName));
        }
    }

    private DeferredLogger AddDeferredLogger(string categoryName)
    {
        var logger = new DeferredLogger(categoryName);
        _deferredLoggers.Add(logger);
        return logger;
    }

    public void Dispose() { }

    public void AddProvider(ILoggerProvider provider)
    {
        _innerLoggerFactory?.AddProvider(provider);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _innerLoggerFactory?.CreateLogger(categoryName) ?? AddDeferredLogger(categoryName);
    }
}

file class DeferredLogger(string categoryName) : ILogger
{
    private readonly List<(LogLevel logLevel, EventId eventId, string text)> _deferredLogs = [];
    private ILogger? _logger;
    public string CategoryName { get; } = categoryName;

    public void SetLogger(ILogger logger)
    {
        _logger = logger;
        foreach (var log in _deferredLogs)
        {
            #pragma warning disable CA1848
            #pragma warning disable CA2254
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.Log(log.logLevel, log.eventId, log.text);
            #pragma warning restore CA2254
            #pragma warning restore CA1848
        }
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (_logger is null)
        {
            _deferredLogs.Add(( logLevel, eventId, formatter(state, exception) ));
            return;
        }

        _logger.Log(logLevel, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger?.IsEnabled(logLevel) ?? true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger?.BeginScope(state);
    }
}

class ProgramInstanceConvention
(
    IProjectMetadata projectMetadata,
    ILoggerFactory loggerFactory,
    FrozenDictionary<string, string> environmentVariables
) : IServiceConvention, IConfigurationConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IConfigurationBuilder builder)
    {
        var projectDirectory = Path.GetDirectoryName(projectMetadata.ProjectPath);
        var wwwRoot = Path.Combine(projectDirectory, "wwwroot");
        if (context.Get<IHostEnvironment>() is { } hostEnvironment)
        {
            hostEnvironment.ContentRootPath = projectDirectory;
            hostEnvironment.ContentRootFileProvider = new PhysicalFileProvider(projectDirectory);
            if (hostEnvironment is IWebHostEnvironment webEnvironment)
            {
                webEnvironment.WebRootPath = wwwRoot;
                webEnvironment.WebRootFileProvider = Directory.Exists(wwwRoot) ? new PhysicalFileProvider(wwwRoot) : new NullFileProvider();
            }
        }

        builder
           .AddInMemoryCollection(
                [
                    new(HostDefaults.ContentRootKey, projectDirectory),
                    new(WebHostDefaults.WebRootKey, wwwRoot),
                ]
            )
           .SetBasePath(projectDirectory);

        builder.AddInMemoryCollection(environmentVariables.Select(z => new KeyValuePair<string, string?>(z.Key.Replace("__", ":"), z.Value)));
    }

    void IServiceConvention.Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton(loggerFactory);
        services.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All);
        services.AddW3CLogging(options => options.LoggingFields = W3CLoggingFields.All);
    }
}

file class AspireLoggerProvider(ILogger logger) : ILoggerProvider
{
    public void Dispose()
    {

    }

    public ILogger CreateLogger(string categoryName)
    {
        return logger;
    }
}

file class HostedStartedService(IHost host, TaskCompletionSource<IHost> tcs) : IHostedLifecycleService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        tcs.SetResult(host);
        return Task.CompletedTask;
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
