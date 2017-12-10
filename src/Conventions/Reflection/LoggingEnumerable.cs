using System;
using System.Collections;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Reflection
{
    static class LoggingEnumerable
    {
        public static IEnumerable<T> Create<T>(IEnumerable<T> enumerable, Action<T> logAction) =>
            new LoggingEnumerable<T>(enumerable, logAction);
    }

    class LoggingEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;
        private readonly Action<T> _logAction;

        public LoggingEnumerable(IEnumerable<T> enumerable, Action<T> logAction)
        {
            _enumerable = enumerable;
            _logAction = logAction;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LoggingEnumerator<T>(_enumerable.GetEnumerator(), _logAction);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
