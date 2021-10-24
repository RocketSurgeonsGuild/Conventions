using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions;

internal delegate IAssemblyCandidateFinder AssemblyCandidateFinderFactory(object? source, ILogger? logger);
