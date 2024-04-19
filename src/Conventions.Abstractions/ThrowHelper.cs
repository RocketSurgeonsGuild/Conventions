using System.Reflection;

namespace Rocket.Surgery.Conventions;

internal static class ThrowHelper
{
    public static Type EnsureTypeIsConvention(Type type)
    {
        if (!typeof(IConvention).IsAssignableFrom(type)) throw new NotSupportedException("Type must inherit from " + nameof(IConvention));

        return type;
    }

    public static TypeInfo EnsureTypeIsConvention(TypeInfo type)
    {
        if (!typeof(IConvention).IsAssignableFrom(type)) throw new NotSupportedException("Type must inherit from " + nameof(IConvention));

        return type;
    }
}