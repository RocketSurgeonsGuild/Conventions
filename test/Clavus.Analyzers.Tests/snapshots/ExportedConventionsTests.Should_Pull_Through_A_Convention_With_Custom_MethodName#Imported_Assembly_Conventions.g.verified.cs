//HintName: Clavus.Analyzers/Clavus.ClavusAttributesGenerator/Imported_Assembly_Conventions.g.cs
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Clavus;
using Clavus.Infrastructure;

#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
[assembly: System.Reflection.AssemblyMetadata("ClavusConfigurationData.Imports.Namespace", "TestProject.Conventions")]
[assembly: System.Reflection.AssemblyMetadata("ClavusConfigurationData.Imports.ClassName", "Imports")]
[assembly: System.Reflection.AssemblyMetadata("ClavusConfigurationData.Imports.MethodName", "Instance")]
namespace TestProject.Conventions
{
    /// <summary>
    /// The class defined for importing Clavus parts into this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Clavus.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal static partial class Imports
    {
        internal static LoadClavusParts Instance = LoadClavusPartsMethod;
        /// <summary>
        /// The Clavus parts imported into this assembly
        /// </summary>
        private static IEnumerable<IClavusPartMetadata> LoadClavusPartsMethod(ClavusContextBuilder builder)
        {
            foreach (var part in Dep1.Dep1Exports.GetConventions(builder))
                yield return part;
            foreach (var part in Dep2Exports.GetConventions(builder))
                yield return part;
            foreach (var part in SampleDependencyThree.Conventions.Exports.GetConventions(builder))
                yield return part;
            foreach (var part in TestProject.Conventions.Exports.SourceMethod(builder))
                yield return part;
        }
    };
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
