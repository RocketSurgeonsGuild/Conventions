using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.Clavus;

namespace Rocket.Surgery.Clavus.WebAssembly.Hosting;

/// <summary>
///     Delegate HostingConventionAction
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
public delegate void WebAssemblyHostingConvention(IClavusContext context, WebAssemblyHostBuilder builder);
