using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Rocket.Surgery.WebAssembly.Hosting.Internals
{
    internal sealed class WebAssemblyHostEnvironment : IWebAssemblyHostEnvironment
    {
        public WebAssemblyHostEnvironment(string environment, string baseAddress)
        {
            Environment = environment;
            BaseAddress = baseAddress;
        }

        public string Environment { get; }

        public string BaseAddress { get; }
    }
}