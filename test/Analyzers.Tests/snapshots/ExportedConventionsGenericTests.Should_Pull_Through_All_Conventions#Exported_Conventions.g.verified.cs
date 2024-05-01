//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Exported_Conventions.g.cs
#pragma warning disable CA1822
#pragma warning disable CS8618
#pragma warning disable CS8603
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.Namespace", "Source.Space")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.ClassName", "Exports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.MethodName", "GetConventions")]
[assembly: ExportedConventions(typeof(Contrib1), typeof(Contrib2), typeof(Contrib3), typeof(Contrib4))]
[assembly: Convention(typeof(Contrib2))]
namespace Source.Space
{
    /// <summary>
    /// The class defined for exporting conventions from this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static partial class Exports
    {
        /// <summary>
        /// The conventions exports from this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(ConventionContextBuilder builder)
        {
            yield return new ConventionWithDependencies(new Contrib1(), HostType.Undefined);
            yield return new ConventionWithDependencies(new Contrib2(), HostType.Undefined);
            yield return new ConventionWithDependencies(new Contrib3(), HostType.Undefined);
            yield return new ConventionWithDependencies(new Contrib4(), HostType.Undefined);
        }
    }
}