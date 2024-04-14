using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting;

/// <summary>
///     Delegate HostingConventionAction
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
public delegate void WebAssemblyHostingConvention(IConventionContext context, WebAssemblyHostBuilder builder);