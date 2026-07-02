using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.WebAssembly.Hosting;

/// <summary>
///     Delegate HostingConventionAction
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
/// <param name="cancellationToken">The cancellation token.</param>
public delegate ValueTask WebAssemblyHostingAsyncConvention(IClavusContext context, WebAssemblyHostBuilder builder, CancellationToken cancellationToken);
