#pragma warning disable CA2000

namespace Clavus.Testing;

/// <summary>
///     A convention test host builder
/// </summary>
public static class TestClavusContextBuilderExtensions
{
    /// <summary>
    ///     Use the given content root path
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="contentRootPath"></param>
    /// <returns></returns>
    public static ClavusContextBuilder WithContentRoot(this ClavusContextBuilder builder, string? contentRootPath)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return string.IsNullOrWhiteSpace(contentRootPath) ? builder : builder.Set("ContentRoot", contentRootPath);
    }

    /// <summary>
    ///     Use the specific environment name
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="environmentName">The environment name.</param>
    public static ClavusContextBuilder WithEnvironmentName(this ClavusContextBuilder builder, string environmentName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.Set("EnvironmentName", environmentName);
    }
}
