using Microsoft.Extensions.Hosting;

namespace Clavus.Hosting;

/// <summary>
///     Delegate HostApplicationPart
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
[PublicAPI]
public delegate void HostApplicationPart<in TBuilder>(IClavusContext context, TBuilder builder)
    where TBuilder : IHostApplicationBuilder;
