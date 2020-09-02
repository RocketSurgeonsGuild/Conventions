using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Routing;

namespace Rocket.Surgery.WebAssembly.Hosting.Internals
{
    internal class NoopNavigationInterception : INavigationInterception
    {
        public Task EnableNavigationInterceptionAsync() => Task.CompletedTask;
    }
}