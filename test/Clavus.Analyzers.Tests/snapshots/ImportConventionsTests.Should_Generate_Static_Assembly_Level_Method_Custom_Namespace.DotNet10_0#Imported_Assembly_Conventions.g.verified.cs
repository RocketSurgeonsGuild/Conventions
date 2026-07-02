//HintName: Rocket.Surgery.Clavus.Analyzers/Rocket.Surgery.Clavus.ClavusAttributesGenerator/Imported_Assembly_Conventions.g.cs
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Clavus;

#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ClavusConfigurationData.Imports.Namespace", "Test.My.Namespace")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ClavusConfigurationData.Imports.ClassName", "MyImports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ClavusConfigurationData.Imports.MethodName", "Instance")]
namespace Test.My.Namespace
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Clavus.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal static partial class MyImports
    {
        internal static LoadClavusParts Instance = LoadConventionsMethod;
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        private static IEnumerable<IClavusPartMetadata> LoadConventionsMethod(ClavusContextBuilder builder)
        {
            foreach (var convention in Dep1.Dep1Exports.GetConventions(builder))
                yield return convention;
            foreach (var convention in Dep2Exports.GetConventions(builder))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(builder))
                yield return convention;
        }
    };
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
