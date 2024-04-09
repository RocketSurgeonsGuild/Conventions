using System.Reflection;

namespace Rocket.Surgery.Conventions.Reflection;

[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
record TypeFilter : ITypeFilter
{
    public List<Func<Type, bool>> Filters { get; } = new();

    public ITypeFilter AssignableTo<T>()
    {
        Filters.Add(typeof(T).IsAssignableFrom);
        return this;
    }

    public ITypeFilter AssignableTo(Type type)
    {
        Filters.Add(type.IsAssignableFrom);
        return this;
    }

    public ITypeFilter AssignableToAny(Type type, params Type[] types)
    {
        types = [type, ..types];
        Filters.Add(x => types.Any(y => y.IsAssignableFrom(x)));
        return this;
    }

    public ITypeFilter Suffix(string value, params string[] values)
    {
        values = [value, ..values];
        Filters.Add(x => values.Any(y => x.Name.EndsWith(y)));
        return this;
    }

    public ITypeFilter Postfix(string value, params string[] values)
    {
        values = [value, ..values];
        Filters.Add(x => values.Any(y => x.Name.EndsWith(y)));
        return this;
    }

    public ITypeFilter EndsWith(string value, params string[] values)
    {
        values = [value, ..values];
        Filters.Add(x => values.Any(y => x.Name.EndsWith(y)));
        return this;
    }

    public ITypeFilter Prefix(string value, params string[] values)
    {
        values = [value, ..values];
        Filters.Add(x => values.Any(y => x.Name.StartsWith(y)));
        return this;
    }

    public ITypeFilter Affix(string value, params string[] values)
    {
        values = [value, ..values];
        Filters.Add(x => values.Any(y => x.Name.EndsWith(y)));
        return this;
    }

    public ITypeFilter StartsWith(string value, params string[] values)
    {
        values = [value, ..values];
        Filters.Add(x => values.Any(y => x.Name.StartsWith(y)));
        return this;
    }

    public ITypeFilter Contains(string value, params string[] values)
    {
        values = [value, ..values];
        Filters.Add(x => values.Any(y => x.Name.Contains(y)));
        return this;
    }

    public ITypeFilter Includes(string value, params string[] values)
    {
        values = [value, ..values];
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
        types = [type, ..types];
        Filters.Add(x => types.Any(y => x.Namespace == y.Namespace));
        return this;
    }

    public ITypeFilter InExactNamespaces(string first, params string[] namespaces)
    {
        namespaces = [first, ..namespaces];
        Filters.Add(x => namespaces.Any(y => x.Namespace == y));
        return this;
    }

    public ITypeFilter InNamespaceOf<T>()
    {
        if (typeof(T) is { Namespace: { } @namespace })
            Filters.Add(x => x.Namespace?.StartsWith(@namespace) == true);
        return this;
    }

    public ITypeFilter InNamespaceOf(Type type, params Type[] types)
    {
        types = [type, ..types];
        Filters.Add(x => types.Any(y => y.Namespace is { Length: > 0 } && x.Namespace?.StartsWith(y.Namespace) == true));
        return this;
    }

    public ITypeFilter InNamespaces(string first, params string[] namespaces)
    {
        namespaces = [first, ..namespaces];
        Filters.Add(x => namespaces.Any(y => y is { Length: > 0 } && x.Namespace?.StartsWith(y) == true));
        return this;
    }

    public ITypeFilter NotInNamespaceOf<T>()
    {
        if (typeof(T) is { Namespace: { } @namespace })
            Filters.Add(x => !x.Namespace?.StartsWith(@namespace) == true);
        return this;
    }

    public ITypeFilter NotInNamespaceOf(Type type, params Type[] types)
    {
        types = [type, ..types];
        Filters.Add(x => !types.Any(y => y.Namespace is { Length: > 0 } && x.Namespace?.StartsWith(y.Namespace) == true));
        return this;
    }

    public ITypeFilter NotInNamespaces(string first, params string[] namespaces)
    {
        namespaces = [first, ..namespaces];
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
}