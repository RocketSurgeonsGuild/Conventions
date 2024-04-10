namespace Rocket.Surgery.Conventions.Reflection;

/// <summary>
///     The Compiled Implementation Type Filter
/// </summary>
[PublicAPI]
public interface ITypeFilter
{
    /// <summary>
    ///     Will match all types that are assignable to <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type that should be assignable from the matching types.</typeparam>
    ITypeFilter AssignableTo<T>();

    /// <summary>
    ///     Will match all types that are assignable to the specified <paramref name="type" />.
    /// </summary>
    /// <param name="type">The type that should be assignable from the matching types.</param>
    ITypeFilter AssignableTo(Type type);

    /// <summary>
    ///     Will match all types that are assignable to any of the specified <paramref name="types" />.
    /// </summary>
    /// <param name="type">The first type that should be assignable from the matching types.</param>
    /// <param name="types">The types that should be assignable from the matching types.</param>
    ITypeFilter AssignableToAny(Type type, params Type[] types);

    /// <summary>
    ///     Will not match all types that are assignable to <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type that should be assignable from the matching types.</typeparam>
    ITypeFilter NotAssignableTo<T>();

    /// <summary>
    ///     Will not match all types that are assignable to the specified <paramref name="type" />.
    /// </summary>
    /// <param name="type">The type that should be assignable from the matching types.</param>
    ITypeFilter NotAssignableTo(Type type);

    /// <summary>
    ///     Will not match all types that are assignable to any of the specified <paramref name="types" />.
    /// </summary>
    /// <param name="type">The first type that should be assignable from the matching types.</param>
    /// <param name="types">The types that should be assignable from the matching types.</param>
    ITypeFilter NotAssignableToAny(Type type, params Type[] types);

    /// <summary>
    ///     Will match all types that end with
    /// </summary>
    /// <param name="value"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    ITypeFilter EndsWith(string value, params string[] values);

    /// <summary>
    ///     Will match all types that end with
    /// </summary>
    /// <param name="value"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    ITypeFilter StartsWith(string value, params string[] values);

    /// <summary>
    ///     Will match all types that contain the given values
    /// </summary>
    /// <param name="value"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    ITypeFilter Contains(string value, params string[] values);

    /// <summary>
    ///     Will match all types in the exact same namespace as the type <typeparamref name="T" />
    /// </summary>
    /// <typeparam name="T">The type in the namespace to include</typeparam>
    ITypeFilter InExactNamespaceOf<T>();

    /// <summary>
    ///     Will match all types in the exact same namespace as the type <paramref name="types" />
    /// </summary>
    /// <param name="type">The first type in the namespaces to include.</param>
    /// <param name="types">The types in the namespaces to include.</param>
    ITypeFilter InExactNamespaceOf(Type type, params Type[] types);

    /// <summary>
    ///     Will match all types in the exact same namespace as the type <paramref name="namespaces" />
    /// </summary>
    /// <param name="first">The first namespace to include.</param>
    /// <param name="namespaces">The namespace to include.</param>
    ITypeFilter InExactNamespaces(string first, params string[] namespaces);

    /// <summary>
    ///     Will match all types in the same namespace as the type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">A type inside the namespace to include.</typeparam>
    ITypeFilter InNamespaceOf<T>();

    /// <summary>
    ///     Will match all types in any of the namespaces of the <paramref name="types" /> specified.
    /// </summary>
    /// <param name="type">The first type in the namespaces to include.</param>
    /// <param name="types">The types in the namespaces to include.</param>
    ITypeFilter InNamespaceOf(Type type, params Type[] types);

    /// <summary>
    ///     Will match all types in any of the <paramref name="namespaces" /> specified.
    /// </summary>
    /// <param name="first">The first namespace to include.</param>
    /// <param name="namespaces">The namespaces to include.</param>
    ITypeFilter InNamespaces(string first, params string[] namespaces);

    /// <summary>
    ///     Will match all types outside of the same namespace as the type <typeparamref name="T" />.
    /// </summary>
    ITypeFilter NotInNamespaceOf<T>();

    /// <summary>
    ///     Will match all types outside of all of the namespaces of the <paramref name="types" /> specified.
    /// </summary>
    /// <param name="type">The first type in the namespaces to include.</param>
    /// <param name="types">The types in the namespaces to include.</param>
    ITypeFilter NotInNamespaceOf(Type type, params Type[] types);

    /// <summary>
    ///     Will match all types outside of all of the <paramref name="namespaces" /> specified.
    /// </summary>
    /// <param name="first">The first namespace to include.</param>
    /// <param name="namespaces">The namespaces to include.</param>
    ITypeFilter NotInNamespaces(string first, params string[] namespaces);

    /// <summary>
    ///     Will match all types that has an attribute of type <typeparamref name="T" /> defined.
    /// </summary>
    /// <typeparam name="T">The type of attribute that needs to be defined.</typeparam>
    ITypeFilter WithAttribute<T>() where T : Attribute;

    /// <summary>
    ///     Will match all types that has an attribute of <paramref name="attributeType" /> defined.
    /// </summary>
    /// <param name="attributeType">Type of the attribute.</param>
    ITypeFilter WithAttribute(Type attributeType);

    /// <summary>
    ///     Will match all types that doesn't have an attribute of type <typeparamref name="T" /> defined.
    /// </summary>
    /// <typeparam name="T">The type of attribute that needs to be defined.</typeparam>
    ITypeFilter WithoutAttribute<T>() where T : Attribute;

    /// <summary>
    ///     Will match all types that doesn't have an attribute of <paramref name="attributeType" /> defined.
    /// </summary>
    /// <param name="attributeType">Type of the attribute.</param>
    ITypeFilter WithoutAttribute(Type attributeType);

     /// <summary>
     ///  Will match all types that are of the specified <paramref name="typeFilter" />.
     /// </summary>
     /// <param name="typeFilter"></param>
     /// <param name="typeFilters"></param>
     /// <returns></returns>
    ITypeFilter KindOf(TypeKindFilter typeFilter, params TypeKindFilter[] typeFilters);
    /// <summary>
    ///   Will match all types that are not of the specified <paramref name="typeFilter" />.
    /// </summary>
    /// <param name="typeFilter"></param>
    /// <param name="typeFilters"></param>
    /// <returns></returns>
    ITypeFilter NotKindOf(TypeKindFilter typeFilter, params TypeKindFilter[] typeFilters);
}
