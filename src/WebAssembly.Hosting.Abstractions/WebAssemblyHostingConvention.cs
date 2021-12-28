using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.WebAssembly.Hosting;

/// <summary>
///     Delegate HostingConventionAction
/// </summary>
/// <param name="context">The context.</param>
public delegate void WebAssemblyHostingConvention(IConventionContext context, IWebAssemblyHostBuilder builder);
