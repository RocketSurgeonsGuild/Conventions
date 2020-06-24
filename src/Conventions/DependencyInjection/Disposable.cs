using System;

namespace Rocket.Surgery.Conventions.DependencyInjection
{
    /// <summary>
    /// Disposable.
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal class Disposable : IDisposable
    {
        private readonly Action _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public Disposable(Action action) => _action = action;

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose() => _action();
    }
}