using System;
using System.Collections;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// LoggingEnumerator.
    /// </summary>
    static class LoggingEnumerator
    {
        /// <summary>
        /// Creates the specified enumerator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="logAction">The log action.</param>
        /// <returns>IEnumerator&lt;T&gt;.</returns>
        public static IEnumerator<T> Create<T>(IEnumerator<T> enumerator, Action<T> logAction) =>
            new LoggingEnumerator<T>(enumerator, logAction);
    }

    /// <summary>
    /// LoggingEnumerator.
    /// Implements the <see cref="System.Collections.Generic.IEnumerator{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerator{T}" />
    class LoggingEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly Action<T> _logAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingEnumerator{T}" /> class.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="logAction">The log action.</param>
        public LoggingEnumerator(IEnumerator<T> enumerator, Action<T> logAction)
        {
            _enumerator = enumerator;
            _logAction = logAction;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            var result = _enumerator.MoveNext();
            if (result) _logAction(Current);
            return result;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _enumerator.Reset();
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The current.</value>
        public T Current => _enumerator.Current;

        object IEnumerator.Current => Current;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}
