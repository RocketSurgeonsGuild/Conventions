using System.Reactive.Linq;
using System.Reflection;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public static class AssemblyResourceExtensions
{
    public static AssemblyResourceViewModel GetAssemblyResource(this DistributedApplication application, string name)
    {
        var model = application.Services.GetRequiredService<DistributedApplicationModel>();
        var resource = model
                      .Resources.OfType<AssemblyResource>()
                      .Single(z => z.Name == name);

        return new(resource, application.ObserveResourceLogs(name));
    }

    public static async Task<IResourceBuilder<AssemblyResource>> RunInMemory<TProject, TEntryPoint>(
        this Task<IDistributedApplicationTestingBuilder> builderTask
    )
        where TProject : class, IProjectMetadata, new()
        where TEntryPoint : class
    {
        return RunInMemory<TProject>(await builderTask, typeof(TEntryPoint).Assembly);
    }

    public static async Task<IResourceBuilder<AssemblyResource>> RunInMemory<TProject>(
        this Task<IDistributedApplicationTestingBuilder> builderTask,
        Assembly assembly
    )
        where TProject : class, IProjectMetadata, new()
    {
        return RunInMemory<TProject>(await builderTask, assembly);
    }

    public static IResourceBuilder<AssemblyResource> RunInMemory<TProject, TEntryPoint>(this IDistributedApplicationTestingBuilder builder)
        where TProject : class, IProjectMetadata, new()
        where TEntryPoint : class
    {
        return RunInMemory<TProject>(builder, typeof(TEntryPoint).Assembly);
    }

    public static IResourceBuilder<AssemblyResource> RunInMemory<TProject>(this IDistributedApplicationTestingBuilder builder, Assembly assembly)
        where TProject : class, IProjectMetadata, new()
    {
        var projectResource = builder
                             .Resources
                             .OfType<ProjectResource>()
                             .SingleOrDefault(z => z.TryGetAnnotationsOfType<TProject>(out _))
         ?? throw new ArgumentException("Project not found!");

        builder.Services.TryAddLifecycleHook<AssemblyResourceLifecycle>();

        builder.Resources.Remove(projectResource);

        return builder.AddResource(
            new AssemblyResource(projectResource, assembly.EntryPoint ?? throw new ArgumentNullException("No entry point found!"))
        );
    }

    public static IResourceBuilder<AssemblyResource> AddExtension(this IResourceBuilder<AssemblyResource> builder, ITestConvention extension)
    {
        builder.Resource.Extensions.Add(extension);
        return builder;
    }

    public static IObservable<LogLine> ObserveResourceLogs(this DistributedApplication application, string? resourceName = null)
    {
        var resourceLoggerService = application.Services.GetRequiredService<ResourceLoggerService>();
        var model = application.Services.GetRequiredService<DistributedApplicationModel>();
        var resources = model.Resources.AsEnumerable();
        if (resourceName != null)
            resources = resources.Where(z => z.Name == resourceName);

        return resources
              .ToObservable()
              .Select(resource => resourceLoggerService.WatchAsync(resource).ToObservable())
              .Merge()
              .SelectMany(z => z);
    }

    public static IObservable<ResourceEvent> ObserveResourceEvents(this DistributedApplication application, string? resourceName = null)
    {
        var resourceNotificationService = application.Services.GetRequiredService<ResourceNotificationService>();
        var obs = resourceNotificationService.WatchAsync().ToObservable();
        return resourceName is null ? obs : obs.Where(z => z.Resource.Name == resourceName);
    }
}

public class AssemblyResourceViewModel : IResourceWithEnvironment, IResourceWithArgs, IResourceWithServiceDiscovery, IDisposable
{
    private readonly AssemblyResource _assemblyResource;
    private readonly IObservable<LogLine> _logs;
    private readonly List<LogLine> _logLines = new();
    private readonly IDisposable _disposable;

    public AssemblyResourceViewModel(AssemblyResource assemblyResource,
        IObservable<LogLine> logs)
    {
        _assemblyResource = assemblyResource;
        _logs = logs;
        _disposable = logs.Subscribe(z => _logLines.Add(z));
    }

    public string Name => _assemblyResource.Name;
    public ResourceAnnotationCollection Annotations => _assemblyResource.Annotations;
    public IObservable<LogLine> ObserveLogs() => _logs;
    public IReadOnlyList<LogLine> Logs => _logLines;
    public void Dispose() => _disposable.Dispose();
}
