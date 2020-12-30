using System;

namespace Sample.Core
{
    internal class AService : IService
    {
        public string GetString() => nameof(AService);
    }
}
