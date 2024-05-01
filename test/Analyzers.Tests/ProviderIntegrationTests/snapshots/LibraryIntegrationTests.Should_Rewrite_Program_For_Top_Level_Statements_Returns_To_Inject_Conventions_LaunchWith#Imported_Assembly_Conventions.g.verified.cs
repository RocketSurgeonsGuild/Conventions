﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Imported_Assembly_Conventions.g.cs
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

#pragma warning disable CA1822
#pragma warning disable CS8618
#pragma warning disable CS8603
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.Namespace", "TestProject.Conventions")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.ClassName", "Imports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.MethodName", "Instance")]
namespace TestProject.Conventions
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed partial class Imports : IConventionFactory
    {
        public static IConventionFactory Instance { get; } = new Imports().OrCallerConventions();

        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public IEnumerable<IConventionWithDependencies> LoadConventions(ConventionContextBuilder builder)
        {
            yield break;
        }
    }
}