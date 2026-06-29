using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     Delegate HostApplicationConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
/// <param name="cancellationToken">The cancellation token.</param>
[PublicAPI]
public delegate ValueTask HostApplicationAsyncConvention<in TBuilder>(IConventionContext context, TBuilder builder, CancellationToken cancellationToken)
    where TBuilder : IHostApplicationBuilder;
