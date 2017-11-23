#if NETSTANDARD1_3 || NET451
using System;

namespace Microsoft.Extensions.Logging.Abstractions
{
    /// <summary>An empty scope without any logic</summary>
    class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
#endif
