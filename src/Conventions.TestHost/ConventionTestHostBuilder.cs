using System;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions.TestHost
{
    // /// <summary>
    // /// A convention test host builder
    // /// </summary>
    // public class ConventionTestHostBuilder
    // {
    //     private IServiceProviderDictionary ServiceProperties = new ServiceProviderDictionary();
    //     private IConventionScanner? Scanner;
    //     private IAssemblyCandidateFinder? AssemblyCandidateFinder;
    //     private IAssemblyProvider? AssemblyProvider;
    //     private DiagnosticSource DiagnosticSource = new DiagnosticListener("ConventionTestHostBuilder.DiagnosticListener");

    //     /// <summary>
    //     /// Withes the specified scanner.
    //     /// </summary>
    //     /// <param name="scanner">The scanner.</param>
    //     /// <returns>ConventionTestHostBuilder.</returns>
    //     public ConventionTestHostBuilder With(IConventionScanner scanner)
    //     {
    //         Scanner = scanner;
    //         return this;
    //     }

    //     /// <summary>
    //     /// Withes the specified assembly candidate finder.
    //     /// </summary>
    //     /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
    //     /// <returns>ConventionTestHostBuilderBuilder.</returns>
    //     public ConventionTestHostBuilder With(IAssemblyCandidateFinder assemblyCandidateFinder)
    //     {
    //         AssemblyCandidateFinder = assemblyCandidateFinder;
    //         return this;
    //     }

    //     /// <summary>
    //     /// Withes the specified assembly provider.
    //     /// </summary>
    //     /// <param name="assemblyProvider">The assembly provider.</param>
    //     /// <returns>ConventionTestHostBuilderBuilder.</returns>
    //     public ConventionTestHostBuilder With(IAssemblyProvider assemblyProvider)
    //     {
    //         AssemblyProvider = assemblyProvider;
    //         return this;
    //     }

    //     /// <summary>
    //     /// Withes the specified diagnostic source.
    //     /// </summary>
    //     /// <param name="diagnosticSource">The diagnostic source.</param>
    //     /// <returns>ConventionTestHostBuilderBuilder.</returns>
    //     public ConventionTestHostBuilder With(DiagnosticSource diagnosticSource)
    //     {
    //         DiagnosticSource = diagnosticSource;
    //         return this;
    //     }

    //     /// <summary>
    //     /// Withes the specified diagnostic source.
    //     /// </summary>
    //     /// <param name="serviceProperties">The diagnostic source.</param>
    //     /// <returns>ConventionTestHostBuilderBuilder.</returns>
    //     public ConventionTestHostBuilder With(IServiceProviderDictionary serviceProperties)
    //     {
    //         ServiceProperties = serviceProperties;
    //         return this;
    //     }

    //     /// <summary>
    //     /// Create the convention test host with the given defaults
    //     /// </summary>
    //     /// <returns></returns>
    //     public ConventionTestHost Create()
    //     {
    //         var assemblyCandidateFinder = AssemblyCandidateFinder ?? throw new ArgumentNullException("AssemblyCandidateFinder");
    //         var assemblyProvider = AssemblyProvider ?? throw new ArgumentNullException("AssemblyCandidateFinder");
    //         return new ConventionTestHost(
    //             Scanner ?? new SimpleConventionScanner(assemblyCandidateFinder, ServiceProperties, new DiagnosticLogger(DiagnosticSource)),
    //             assemblyCandidateFinder,
    //             assemblyProvider,
    //             DiagnosticSource,
    //             ServiceProperties
    //         );
    //     }
    // }
}
