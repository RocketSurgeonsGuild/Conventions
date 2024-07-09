using System.Collections.ObjectModel;
using System.Reflection;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public class AssemblyResource : Resource, IResourceWithEnvironment, IResourceWithArgs, IResourceWithServiceDiscovery
{
    public ProjectResource Project { get; }
    public MethodInfo EntryPoint { get; }
    public Func<ConventionContextBuilder, CancellationToken, ValueTask> Configure { get; }

    public Collection<ITestConvention> Extensions { get; } = new();

    public AssemblyResource(
        ProjectResource project,
        MethodInfo entryPoint,
        Func<ConventionContextBuilder, CancellationToken, ValueTask>? configure = null
    ) : base(project.Name)
    {
        Project = project;
        EntryPoint = entryPoint;
        Configure = configure ?? ( (_, _) => ValueTask.CompletedTask );
    }
}