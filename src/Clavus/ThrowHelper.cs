using System.Reflection;

namespace Clavus;

internal static class ThrowHelper
{
    public static Type EnsureTypeIsConvention(Type type)
    {
        return  !typeof(IClavusPart).IsAssignableFrom(type)
            ? throw new NotSupportedException("Type must inherit from " + nameof(IClavusPart))
            : type;
    }

    public static TypeInfo EnsureTypeIsConvention(TypeInfo type)
    {
        return  !typeof(IClavusPart).IsAssignableFrom(type)
            ? throw new NotSupportedException("Type must inherit from " + nameof(IClavusPart))
            : type;
    }
}
