using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions
{
    internal static class ConventionContextHelpers
    {
        private static readonly ConcurrentDictionary<Assembly, List<IConvention>> Conventions =
            new ConcurrentDictionary<Assembly, List<IConvention>>();

        static IEnumerable<IConvention> GetAssemblyConventions(
            ConventionContextBuilder builder,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            ILogger? logger
        )
        {
            var assemblies = assemblyCandidateFinder.GetCandidateAssemblies(
                "Rocket.Surgery.Conventions.Abstractions",
                "Rocket.Surgery.Conventions.Attributes",
                "Rocket.Surgery.Conventions"
            ).ToArray();

            var prependedConventionTypes = new Lazy<HashSet<Type>>(
                () => new HashSet<Type>(
                    builder._prependedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct()
                )
            );
            var appendedConventionTypes = new Lazy<HashSet<Type>>(
                () => new HashSet<Type>(
                    builder._appendedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct()
                )
            );

            if (logger?.IsEnabled(LogLevel.Debug) == true)
            {
                logger.LogDebug("Scanning for conventions in assemblies: {Assemblies}", assemblies.Select(x => x.GetName().Name));
                if (builder._exceptAssemblyConventions.Count > 0)
                {
                    logger.LogDebug(
                        "Skipping conventions in assemblies: {Assemblies}",
                        builder._exceptAssemblyConventions.Select(x => x.GetName().Name)
                    );
                }

                logger.LogDebug(
                    "Skipping existing convention types: {Types}",
                    prependedConventionTypes.Value.Concat(appendedConventionTypes.Value).Select(x => x.FullName)
                );
            }

            foreach (var assembly in assemblies.Except(builder._exceptAssemblyConventions))
            {
                if (!Conventions.TryGetValue(assembly, out var types))
                {
                    types = assembly.GetCustomAttributes<ConventionAttribute>()
                       .Select(x => x.Type)
                       .Distinct()
                       .Select(type => ActivatorUtilities.CreateInstance(builder.Properties, type))
                       .Cast<IConvention>()
                       .ToList();
                    Conventions.TryAdd(assembly, types);
                }
                else if (logger?.IsEnabled(LogLevel.Debug) == true)
                {
                    logger.LogDebug(
                        "Conventions from Assembly {Assembly} have already been scanned and activated!",
                        assembly.GetName().Name
                    );
                }

                if (logger?.IsEnabled(LogLevel.Debug) == true)
                {
                    logger.LogDebug(
                        "Found conventions in Assembly {Assembly} ({@Conventions})",
                        assembly.GetName().Name,
                        types.Select(z => z.GetType().FullName)
                    );
                }

                foreach (var item in types
                   .Select(
                        x =>
                        {
                            if (logger?.IsEnabled(LogLevel.Trace) == true)
                            {
                                logger.LogTrace(
                                    "Scanning => Prefilter: {Assembly} / {Type}",
                                    assembly.GetName().Name,
                                    x.GetType().FullName
                                );
                            }

                            return x;
                        }
                    )
                   .Where(
                        type => !prependedConventionTypes.Value.Contains(type.GetType()) &&
                            !appendedConventionTypes.Value.Contains(type.GetType())
                    )
                   .Select(
                        x =>
                        {
                            if (logger?.IsEnabled(LogLevel.Trace) == true)
                            {
                                logger.LogTrace(
                                    "Scanning => Postfilter: {Assembly} / {Type}",
                                    assembly.GetName().Name,
                                    x.GetType().FullName
                                );
                            }

                            return x;
                        }
                    ))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Method used to create a convention provider
        /// </summary>
        /// <returns></returns>
        internal static IConventionProvider CreateProvider(
            ConventionContextBuilder builder,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            ILogger? logger
        )
        {
            for (var i = 0; i < builder._prependedConventions.Count; i++)
            {
                if (builder._prependedConventions[i] is Type type)
                {
                    builder._prependedConventions[i] = ActivatorUtilities.CreateInstance(builder.Properties, type);
                }
            }

            for (var i = 0; i < builder._appendedConventions.Count; i++)
            {
                if (builder._appendedConventions[i] is Type type)
                {
                    builder._appendedConventions[i] = ActivatorUtilities.CreateInstance(builder.Properties, type);
                }
            }

            var contributionTypes = builder._useAttributeConventions
                ? GetAssemblyConventions(builder, assemblyCandidateFinder, logger)
                   .Where(z => builder._exceptConventions.All(x => x != z.GetType()))
                : Enumerable.Empty<IConvention>();

            return new ConventionProvider(builder.GetHostType(), contributionTypes, builder._prependedConventions, builder._appendedConventions);
        }

        internal static IAssemblyCandidateFinder defaultAssemblyCandidateFinderFactory(object? source, ILogger? logger) => source switch
        {
            AppDomain appDomain              => new AppDomainAssemblyCandidateFinder(appDomain, logger),
            IEnumerable<Assembly> assemblies => new DefaultAssemblyCandidateFinder(assemblies, logger),
            _                                => throw new NotSupportedException("Unknown source when trying to create IAssemblyCandidateFinder")
        };

        internal static IAssemblyProvider defaultAssemblyProviderFactory(object? source, ILogger? logger) => source switch
        {
            AppDomain appDomain              => new AppDomainAssemblyProvider(appDomain, logger),
            IEnumerable<Assembly> assemblies => new DefaultAssemblyProvider(assemblies, logger),
            _                                => throw new NotSupportedException("Unknown source when trying to create IAssemblyCandidateFinder")
        };
    }
}