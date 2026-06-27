using System.Reflection;

namespace Rocket.Surgery.Conventions;

internal static class ThrowHelper
{
    public static Type EnsureTypeIsConvention(Type type)
    {
        return !typeof(IConvention).IsAssignableFrom(type)
            ? throw new NotSupportedException("Type must inherit from " + nameof(IConvention))
            : type;
    }

    public static TypeInfo EnsureTypeIsConvention(TypeInfo type)
    {
        return !typeof(IConvention).IsAssignableFrom(type)
            ? throw new NotSupportedException("Type must inherit from " + nameof(IConvention))
            : type;
    }
}
