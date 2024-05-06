using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     Delegate HostApplicationConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
[PublicAPI]
public delegate void HostApplicationConvention<in TBuilder>(IConventionContext context, TBuilder builder)
    where TBuilder : IHostApplicationBuilder;