using System;
using System.Collections;
using System.Collections.Generic;

namespace Rocket.Surgery.Conventions.Reflection
{
    static class LoggingEnumerator
    {
        public static IEnumerator<T> Create<T>(IEnumerator<T> enumerator, Action<T> logAction) =>
            new LoggingEnumerator<T>(enumerator, logAction);
    }

    class LoggingEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly Action<T> _logAction;

        public LoggingEnumerator(IEnumerator<T> enumerator, Action<T> logAction)
        {
            _enumerator = enumerator;
            _logAction = logAction;
        }

        public bool MoveNext()
        {
            var result = _enumerator.MoveNext();
            if (result) _logAction(Current);
            return result;
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public T Current => _enumerator.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}
