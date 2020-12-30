using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Assembly candidate finder that uses <see cref="AppDomain" />
    /// </summary>
    /// <seealso cref="IAssemblyCandidateFinder" />
    internal class AppDomainAssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        private readonly AppDomain _appDomain;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContextAssemblyCandidateFinder" /> class.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="logger">The logger.</param>
        public AppDomainAssemblyCandidateFinder(AppDomain? appDomain = null, ILogger? logger = null)
        {
            _appDomain = appDomain ?? AppDomain.CurrentDomain;
            _logger = logger ?? NullLogger.Instance;
        }

        private Action<Assembly> LogValue(string[] candidates) => value =>
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "[{AssemblyCandidateFinder}] Found candidate assembly {AssemblyName} for candidates {@Candidates}",
                    nameof(AppDomainAssemblyCandidateFinder),
                    value.GetName().Name,
                    candidates
                );
            }
        };

        private IEnumerable<Assembly> GetCandidateLibraries(string[] candidates)
        {
            if (candidates?.Any() != true)
            {
                return Enumerable.Empty<Assembly>();
            }

            // Sometimes all the assemblies are not loaded... so we kind of have to yolo it and try a few times until we get all of them
            var candidatesResolver = new AssemblyCandidateResolver(
                _appDomain.GetAssemblies(),
                new HashSet<string?>(candidates, StringComparer.OrdinalIgnoreCase),
                _logger
            );
            return candidatesResolver.GetCandidates().Select(x => x.Assembly).ToArray();
        }

        /// <summary>
        /// Get the candidates for a given set
        /// </summary>
        /// <param name="candidates">The candidates as an enumerable</param>
        /// <returns>IEnumerable{Assembly}.</returns>
        public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates)
        {
            var value = candidates as string[] ?? candidates.ToArray();
            return LoggingEnumerable.Create(
                GetCandidateLibraries(value)
                   .Where(x => x != null)
                   .Reverse(),
                LogValue(value)
            );
        }
    }
}