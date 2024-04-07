#if NET6_0_OR_GREATER
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
public delegate ValueTask HostApplicationAsyncConvention(IConventionContext context, IHostApplicationBuilder builder, CancellationToken cancellationToken);
#endif