﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Imported_Assembly_Conventions.g.cs
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

#pragma warning disable CA1822
#pragma warning disable CS8618
#pragma warning disable CS8603
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.Namespace", "Test.My.Namespace")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.ClassName", "MyImports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.MethodName", "ImportConventions")]
namespace Test.My.Namespace
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed partial class MyImports : IConventionFactory
    {
        public static IConventionFactory ImportConventions { get; } = new MyImports().OrCallerConventions();

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