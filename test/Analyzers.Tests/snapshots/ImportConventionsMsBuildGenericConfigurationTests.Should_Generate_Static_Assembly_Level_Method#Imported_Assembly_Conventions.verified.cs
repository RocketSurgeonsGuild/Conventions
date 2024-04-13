//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Imported_Assembly_Conventions.cs
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.Namespace", "TestProject.Conventions")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.ClassName", "Imports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.MethodName", "GetConventions")]
namespace TestProject.Conventions
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal partial class Imports : IConventionFactory
    {
        public static Imports GetConventions { get; } = new Imports();

        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public IEnumerable<IConventionWithDependencies> LoadConventions(ConventionContextBuilder builder)
        {
            foreach (var convention in Dep1.Dep1Exports.GetConventions(builder))
                yield return convention;
            foreach (var convention in Dep2Exports.GetConventions(builder))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(builder))
                yield return convention;
        }
    }
}