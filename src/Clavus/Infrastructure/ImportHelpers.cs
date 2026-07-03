using System.ComponentModel;
using System.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Clavus.Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ImportHelpers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static LoadClavusParts? ExternalConventions { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static LoadClavusParts OrCallerConventions(this LoadClavusParts loader)
    {
        ArgumentNullException.ThrowIfNull(loader);
        return Assembly.GetEntryAssembly() is not { } entryAssembly || loader.GetType().Assembly == entryAssembly || ExternalConventions == null
            ? loader
            : ExternalConventions;
    }
}
