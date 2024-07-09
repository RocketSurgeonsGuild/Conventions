using System.ComponentModel;
using System.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Rocket.Surgery.Conventions;

[EditorBrowsable(EditorBrowsableState.Never)]
[AttributeUsage(AttributeTargets.Assembly)]
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
    public static void SetExternalConfigureMethodWithLock(Func<ConventionContextBuilder, CancellationToken, ValueTask> configure, CancellationToken cancellationToken = default)
    {
//        _lock.Wait(cancellationToken);
//        externalConfigureMethod = configure;
        throw new NotImplementedException("Review later");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async ValueTask RunExternalConfigureMethod(ConventionContextBuilder builder, CancellationToken cancellationToken = default)
    {
        try
        {
            if (externalConfigureMethod is { }) await externalConfigureMethod(builder, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IConventionFactory? CallerConventions(Assembly callerAssembly)
    {
        return Assembly.GetEntryAssembly() is not { } entryAssembly || callerAssembly == entryAssembly || externalConventions == null
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
    private static Func<ConventionContextBuilder, CancellationToken, ValueTask>? externalConfigureMethod;
    private static readonly SemaphoreSlim _lock = new(1, 1);
}
