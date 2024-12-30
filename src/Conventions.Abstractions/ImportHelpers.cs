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
    public static LoadConventions? ExternalConventions
    {
        get => externalLoader;
        set => externalLoader = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static LoadConventions OrCallerConventions(this LoadConventions loader)
    {
        ArgumentNullException.ThrowIfNull(loader);
        return Assembly.GetEntryAssembly() is not { } entryAssembly || loader.GetType().Assembly == entryAssembly || externalLoader == null
            ? loader
            : externalLoader;
    }

    private static LoadConventions? externalLoader;
}
