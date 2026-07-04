using PropertiesType = System.Collections.Generic.IDictionary<object, object>;

namespace Clavus.Infrastructure;


/// <summary>
///   A delegate that can be used to create a <see cref="ClavusContextBuilder" />.
/// </summary>
/// <param name="properties">The properties to initialize the context builder with.</param>
/// <param name="categories">The categories to initialize the context builder with.</param>
/// <returns>A new instance of <see cref="ClavusContextBuilder" />.</returns>
public delegate ClavusContextBuilder ClavusContextBuilderFactory(PropertiesType? properties = null, IEnumerable<ClavusCategory>? categories = null);
