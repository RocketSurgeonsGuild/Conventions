using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions
{
    internal delegate IAssemblyProvider AssemblyProviderFactory(object? source, ILogger? logger);
}