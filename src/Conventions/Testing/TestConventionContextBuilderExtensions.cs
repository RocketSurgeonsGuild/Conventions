using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions.Testing;

/// <summary>
///     A convention test host builder
/// </summary>
public static class TestConventionContextBuilderExtensions
{
    /// <summary>
    ///     Use the given content root path
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="contentRootPath"></param>
    /// <returns></returns>
    public static ConventionContextBuilder WithContentRoot(this ConventionContextBuilder builder, string? contentRootPath)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return string.IsNullOrWhiteSpace(contentRootPath) ? builder : builder.Set("ContentRoot", contentRootPath);
    }

    /// <summary>
    ///     Use the specific environment name
    /// </summary>
    /// <param name="builder">The convention context builder.</param>
    /// <param name="environmentName">The environment name.</param>
    public static ConventionContextBuilder WithEnvironmentName(this ConventionContextBuilder builder, string environmentName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.Set("EnvironmentName", environmentName);
    }
}
