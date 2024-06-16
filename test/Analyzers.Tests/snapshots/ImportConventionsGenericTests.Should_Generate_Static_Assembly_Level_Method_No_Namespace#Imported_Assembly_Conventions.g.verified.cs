//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Imported_Assembly_Conventions.g.cs
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

#nullable enable
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.Namespace", "")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.ClassName", "MyImports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.MethodName", "Instance")]
/// <summary>
/// The class defined for importing conventions into this assembly
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed partial class MyImports : IConventionFactory
{
    public static IConventionFactory Instance { get; } = new MyImports().OrCallerConventions();

    /// <summary>
    /// The conventions imported into this assembly
    /// </summary>
    public IEnumerable<IConventionMetadata> LoadConventions(ConventionContextBuilder builder)
    {
        foreach (var convention in Dep1.Dep1Exports.GetConventions(builder))
            yield return convention;
        foreach (var convention in Dep2Exports.GetConventions(builder))
            yield return convention;
        foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(builder))
            yield return convention;
    }
}
#pragma warning restore CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
#nullable restore
