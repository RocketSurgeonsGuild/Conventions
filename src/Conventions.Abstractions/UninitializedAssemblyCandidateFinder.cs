using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions
{
    class UninitializedAssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates) => throw new NotImplementedException();
    }
}