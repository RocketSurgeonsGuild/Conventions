using System;
using System.Collections;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// LoggingEnumerable.
    /// </summary>
    internal static class LoggingEnumerable
    {
        /// <summary>
        /// Creates the specified enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="logAction">The log action.</param>
        /// <returns>IEnumerable{T}.</returns>
        public static IEnumerable<T> Create<T>(IEnumerable<T> enumerable, Action<T> logAction)
            => new LoggingEnumerable<T>(enumerable, logAction);
    }

    /// <summary>
    /// LoggingEnumerable.
    /// Implements the <see cref="IEnumerable{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IEnumerable{T}" />
    internal class LoggingEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;
        private readonly Action<T> _logAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingEnumerable{T}" /> class.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="logAction">The log action.</param>
        public LoggingEnumerable(IEnumerable<T> enumerable, Action<T> logAction)
        {
            _enumerable = enumerable;
            _logAction = logAction;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => new LoggingEnumerator<T>(_enumerable.GetEnumerator(), _logAction);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}