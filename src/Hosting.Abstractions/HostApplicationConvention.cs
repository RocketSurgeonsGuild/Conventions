#if NET8_0_OR_GREATER
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting;

/// <summary>
///     Delegate HostApplicationConvention
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
public delegate void HostApplicationConvention(IConventionContext context, IHostApplicationBuilder builder);
#endif
