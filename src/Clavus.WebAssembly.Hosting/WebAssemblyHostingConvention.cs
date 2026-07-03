using Clavus;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Clavus.WebAssembly.Hosting;

/// <summary>
///     Delegate HostingConventionAction
/// </summary>
/// <param name="context">The context.</param>
/// <param name="builder">The builder.</param>
public delegate void WebAssemblyHostingConvention(IClavusContext context, WebAssemblyHostBuilder builder);
