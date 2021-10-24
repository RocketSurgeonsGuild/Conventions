using Microsoft.AspNetCore.Components;

namespace Rocket.Surgery.WebAssembly.Hosting.Internals;

internal class NoopNavigationManager : NavigationManager
{
    protected override void NavigateToCore(string uri, bool forceLoad)
    {
    }
}
