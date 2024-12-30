//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Imported_Assembly_Conventions.g.cs
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.Namespace", "")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.ClassName", "MyImports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.MethodName", "Instance")]
/// <summary>
/// The class defined for importing conventions into this assembly
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static partial class MyImports
{
    internal static LoadConventions Instance = LoadConventionsMethod;
    /// <summary>
    /// The conventions imported into this assembly
    /// </summary>
    private static IEnumerable<IConventionMetadata> LoadConventionsMethod(ConventionContextBuilder builder)
    {
        foreach (var convention in Dep1.Dep1Exports.GetConventions(builder))
            yield return convention;
        foreach (var convention in Dep2Exports.GetConventions(builder))
            yield return convention;
        foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(builder))
            yield return convention;
    }
};
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
