using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Rocket.Surgery.WebAssembly.Hosting.Internals
{
    internal class NoopJSRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args) => default;
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object[] args) => default;
    }
}