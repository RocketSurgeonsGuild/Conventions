using System.ComponentModel;
using System.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Rocket.Surgery.Conventions;

[EditorBrowsable(EditorBrowsableState.Never)]
public class ImportsTypeAttribute(Type type) : Attribute
{
    public Type Type { get; } = type;
}

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ImportHelpers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IConventionFactory? ExternalConventions
    {
        set => externalConventions = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IConventionFactory? CallerConventions(Assembly callerAssembly)
    {
        return Assembly.GetEntryAssembly() is not { } entryAssembly  || callerAssembly == entryAssembly || externalConventions == null
            ? null
            : entryAssembly.GetCustomAttribute<ImportsTypeAttribute>()?.Type is { } executingImportsType
         && Activator.CreateInstance(executingImportsType) is IConventionFactory imports
                ? imports
                : externalConventions;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IConventionFactory OrCallerConventions(this IConventionFactory conventionFactory)
    {
        return Assembly.GetEntryAssembly() is not { } entryAssembly || conventionFactory.GetType().Assembly == entryAssembly || externalConventions == null
            ? conventionFactory
            : entryAssembly.GetCustomAttribute<ImportsTypeAttribute>()?.Type is { } executingImportsType
         && Activator.CreateInstance(executingImportsType) is IConventionFactory imports
                ? imports
                : externalConventions;
    }

    private static IConventionFactory? externalConventions;
}
