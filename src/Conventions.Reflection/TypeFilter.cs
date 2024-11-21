using System.Collections.Immutable;
using System.Reflection;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Reflection;

[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
internal record TypeFilter : ITypeFilter
{
    private static bool TypeKindFilterFunc(TypeKindFilter typeFilter, TypeKindFilter filter, Type type) => filter switch
        {
            TypeKindFilter.Array     => type.IsArray,
            TypeKindFilter.Class     => type.IsClass,
            TypeKindFilter.Delegate  => typeof(Delegate).IsAssignableFrom(type),
            TypeKindFilter.Enum      => type.IsEnum,
            TypeKindFilter.Interface => type.IsInterface,
            TypeKindFilter.Struct    => type.IsValueType,
            _                        => throw new ArgumentOutOfRangeException(nameof(typeFilter), typeFilter, null),
        };

    private static bool TypeInfoFilterFunc(TypeInfoFilter filter, Type type) => filter switch
                                                                                {
                                                                                    TypeInfoFilter.Abstract  => type.IsAbstract,
                                                                                    TypeInfoFilter.Visible   => type.IsVisible,
                                                                                    TypeInfoFilter.ValueType => type.IsValueType,
                                                                                    //            TypeInfoFilter.Nested                => type.IsNested,
                                                                                    TypeInfoFilter.Sealed      => type.IsSealed,
                                                                                    TypeInfoFilter.GenericType => type is { IsGenericType: true },
                                                                                    TypeInfoFilter.Unknown     => throw new NotImplementedException(),
                                                                                    //            TypeInfoFilter.GenericTypeDefinition => type.IsGenericTypeDefinition,
                                                                                    _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null),
                                                                                };

    public List<Func<Type, bool>> Filters { get; } = [type => !type.Name.StartsWith("<")];

    public ITypeFilter AssignableTo<T>()
    {
        Filters.Add(x => typeof(T).IsAssignableFrom(x));
        return this;
    }

    public ITypeFilter AssignableTo(Type type)
    {
        Filters.Add(type.IsAssignableFrom);
        return this;
    }

    public ITypeFilter AssignableToAny(Type type, params Type[] types)
    {
        types = [type, .. types];
        Filters.Add(x => types.Any(t => t.IsAssignableFrom(x)));
        return this;
    }

    public ITypeFilter NotAssignableTo<T>()
    {
        Filters.Add(x => !typeof(T).IsAssignableFrom(x));
        return this;
    }

    public ITypeFilter NotAssignableTo(Type type)
    {
        Filters.Add(t => !type.IsAssignableFrom(t));
        return this;
    }

    public ITypeFilter NotAssignableToAny(Type type, params Type[] types)
    {
        types = [type, .. types];
        Filters.Add(x => types.All(t => !t.IsAssignableFrom(x)));
        return this;
    }

    public ITypeFilter EndsWith(string value, params string[] values)
    {
        values = [value, .. values];
        Filters.Add(x => values.Any(y => x.Name.EndsWith(y)));
        return this;
    }

    public ITypeFilter StartsWith(string value, params string[] values)
    {
        values = [value, .. values];
        Filters.Add(x => values.Any(y => x.Name.StartsWith(y)));
        return this;
    }

    public ITypeFilter Contains(string value, params string[] values)
    {
        values = [value, .. values];
        Filters.Add(x => values.Any(y => x.Name.Contains(y)));
        return this;
    }

    public ITypeFilter InExactNamespaceOf<T>()
    {
        Filters.Add(x => x.Namespace == typeof(T).Namespace);
        return this;
    }

    public ITypeFilter InExactNamespaceOf(Type type, params Type[] types)
    {
        types = [type, .. types];
        Filters.Add(x => types.Any(y => x.Namespace == y.Namespace));
        return this;
    }

    public ITypeFilter InExactNamespaces(string first, params string[] namespaces)
    {
        namespaces = [first, .. namespaces];
        Filters.Add(x => namespaces.Any(y => x.Namespace == y));
        return this;
    }

    public ITypeFilter InNamespaceOf<T>()
    {
        if (typeof(T) is { Namespace: { } @namespace })
        {
            Filters.Add(x => x.Namespace?.StartsWith(@namespace) == true);
        }

        return this;
    }

    public ITypeFilter InNamespaceOf(Type type, params Type[] types)
    {
        types = [type, .. types];
        Filters.Add(x => types.Any(y => y.Namespace is { Length: > 0 } && x.Namespace?.StartsWith(y.Namespace) == true));
        return this;
    }

    public ITypeFilter InNamespaces(string first, params string[] namespaces)
    {
        namespaces = [first, .. namespaces];
        Filters.Add(x => namespaces.Any(y => y is { Length: > 0 } && x.Namespace?.StartsWith(y) == true));
        return this;
    }

    public ITypeFilter NotInNamespaceOf<T>()
    {
        if (typeof(T) is { Namespace: { } @namespace })
        {
            Filters.Add(x => x.Namespace?.StartsWith(@namespace) == false);
        }

        return this;
    }

    public ITypeFilter NotInNamespaceOf(Type type, params Type[] types)
    {
        types = [type, .. types];
        Filters.Add(x => !types.Any(y => y.Namespace is { Length: > 0 } && x.Namespace?.StartsWith(y.Namespace) == true));
        return this;
    }

    public ITypeFilter NotInNamespaces(string first, params string[] namespaces)
    {
        namespaces = [first, .. namespaces];
        Filters.Add(x => !namespaces.Any(y => y is { Length: > 0 } && x.Namespace?.StartsWith(y) == true));
        return this;
    }

    public ITypeFilter WithAttribute<T>() where T : Attribute
    {
        Filters.Add(x => x.GetCustomAttribute<T>() is { });
        return this;
    }

    public ITypeFilter WithAttribute(Type attributeType)
    {
        Filters.Add(x => x.GetCustomAttribute(attributeType) is { });
        return this;
    }

    public ITypeFilter WithAttribute(string? attributeFullName)
    {
        Filters.Add(x => x.GetCustomAttributes().Any(z => z.GetType().FullName == attributeFullName));
        return this;
    }

    public ITypeFilter WithAnyAttribute(Type attributeType, params Type[] attributeTypes)
    {
        attributeTypes = [attributeType, .. attributeTypes];
        Filters.Add(
            x => x
                .GetCustomAttributes()
                .Select(
                     z => z.GetType().IsGenericType ? x.GetGenericTypeDefinition() : z.GetType()
                 )
                .Join([attributeType, .. attributeTypes], z => z, z => z, (type, type1) => true)
                .Any()
        );
        return this;
    }

    public ITypeFilter WithAnyAttribute(string? attributeFullName, params string[] attributeFullNames)
    {
        attributeFullNames = [attributeFullName, .. attributeFullNames];
        Filters.Add(
            x => x
                .GetCustomAttributes()
                .Select(
                     z => z.GetType().FullName
                 )
                .Join([attributeFullName, .. attributeFullNames], z => z, z => z, (_, _) => true)
                .Any()
        );
        return this;
    }

    public ITypeFilter WithoutAttribute<T>() where T : Attribute
    {
        Filters.Add(x => x.GetCustomAttribute<T>() is null);
        return this;
    }

    public ITypeFilter WithoutAttribute(Type attributeType)
    {
        Filters.Add(x => x.GetCustomAttribute(attributeType) is null);
        return this;
    }

    public ITypeFilter WithoutAttribute(string? attributeFullName)
    {
        Filters.Add(x => x.GetCustomAttributes().All(z => z.GetType().FullName != attributeFullName));
        return this;
    }

    public ITypeFilter KindOf(TypeKindFilter typeKindFilter, params TypeKindFilter[] typeKindFilters)
    {
        var filters = ImmutableArray.Create([typeKindFilter, .. typeKindFilters]);
        Filters.Add(x => filters.Any(f => TypeKindFilterFunc(typeKindFilter, f, x)));
        return this;
    }

    public ITypeFilter NotKindOf(TypeKindFilter typeKindFilter, params TypeKindFilter[] typeKindFilters)
    {
        var filters = ImmutableArray.Create([typeKindFilter, .. typeKindFilters]);
        Filters.Add(type => !filters.Any(filter => TypeKindFilterFunc(typeKindFilter, filter, type)));
        return this;
    }

    public ITypeFilter InfoOf(TypeInfoFilter typeInfoFilter, params TypeInfoFilter[] typeInfoFilters)
    {
        var filters = ImmutableArray.Create([typeInfoFilter, .. typeInfoFilters]);
        Filters.Add(type => filters.Any(filter => TypeInfoFilterFunc(filter, type)));
        return this;
    }

    public ITypeFilter NotInfoOf(TypeInfoFilter typeFilter, params TypeInfoFilter[] typeFilters)
    {
        var filters = ImmutableArray.Create([typeFilter, .. typeFilters]);
        Filters.Add(type => !filters.Any(filter => TypeInfoFilterFunc(filter, type)));
        return this;
    }
}
