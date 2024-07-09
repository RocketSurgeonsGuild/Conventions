using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Aspire.Hosting.Testing;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public static class AssemblyResourceExtensions
{
    public static async Task<IResourceBuilder<AssemblyResource>> RunInMemory<TProject, TEntryPoint>(
        this Task<IDistributedApplicationTestingBuilder> builderTask
    )
        where TProject : class, IProjectMetadata, new()
        where TEntryPoint : class
    {
        return RunInMemory<TProject, TEntryPoint>(await builderTask);
    }

    public static IResourceBuilder<AssemblyResource> RunInMemory<TProject, TEntryPoint>(this IDistributedApplicationTestingBuilder builder)
        where TProject : class, IProjectMetadata, new()
        where TEntryPoint : class
    {
        var projectResource = builder
                             .Resources
                             .OfType<ProjectResource>()
                             .SingleOrDefault(z => z.TryGetAnnotationsOfType<TProject>(out _))
         ?? throw new ArgumentException("Project not found!");

        builder.Services.TryAddLifecycleHook<AssemblyResourceLifecycle>();

        builder.Resources.Remove(projectResource);

        return builder.AddResource(
            new AssemblyResource(projectResource, typeof(TEntryPoint).Assembly.EntryPoint ?? throw new ArgumentNullException("No entry point found!"))
        );
    }
}