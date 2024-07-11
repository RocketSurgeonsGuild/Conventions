using System.Collections.ObjectModel;
using System.Reflection;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Aspire.Hosting.Testing;

public class AssemblyResource : Resource, IResourceWithEnvironment, IResourceWithArgs, IResourceWithServiceDiscovery
{
    public AssemblyResource(
        ProjectResource project,
        MethodInfo entryPoint,
        Func<ConventionContextBuilder, CancellationToken, ValueTask>? configure = null
    ) : base(project.Name)
    {
        Project = project;
        EntryPoint = entryPoint;
        Configure = configure ?? ( (_, _) => ValueTask.CompletedTask );
        foreach (var item in project.Annotations) Annotations.Add(item);
    }

    internal ProjectResource Project { get; }
    internal MethodInfo EntryPoint { get; }
    internal Func<ConventionContextBuilder, CancellationToken, ValueTask> Configure { get; }

    internal Collection<ITestConvention> Extensions { get; } = new();
}