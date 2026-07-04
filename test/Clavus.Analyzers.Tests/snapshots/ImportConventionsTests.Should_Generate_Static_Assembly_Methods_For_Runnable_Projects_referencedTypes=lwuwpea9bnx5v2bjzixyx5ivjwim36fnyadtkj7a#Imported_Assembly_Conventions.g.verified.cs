//HintName: Clavus.Analyzers/Clavus.ClavusAttributesGenerator/Imported_Assembly_Conventions.g.cs
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Clavus;
using Clavus.Infrastructure;

#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
[assembly: System.Reflection.AssemblyMetadata("Clavus.Import.Property", "Import")]
[assembly: System.Reflection.AssemblyMetadata("Clavus.Import.Namespace", "")]
[assembly: System.Reflection.AssemblyMetadata("Clavus.Import.ClassName", "Imports")]
[assembly: System.Reflection.AssemblyMetadata("Clavus.Import.MethodName", "Ashlar")]
/// <summary>
/// The class defined for importing Clavus parts into this assembly
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("Clavus.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static partial class Imports
{
    internal static ClavusContextBuilderFactory Ashlar = CreateClavusContextBuilder;
    /// <summary>
    /// Creates the context builder populated with the Clavus parts imported into this assembly
    /// </summary>
    private static ClavusContextBuilder CreateClavusContextBuilder(IDictionary<object, object>? properties = null, IEnumerable<ClavusCategory>? categories = null) => ClavusContextBuilder.Create(LoadClavusPartsMethod(), properties ?? new Dictionary<object, object>(), categories ?? []);
    /// <summary>
    /// The Clavus parts imported into this assembly
    /// </summary>
    private static IEnumerable<IClavusPartMetadata> LoadClavusPartsMethod()
    {
        foreach (var part in Dep2Exports.Ashlar())
            yield return part;
        foreach (var part in Dep1.Dep1Exports.Ashlar())
            yield return part;
        foreach (var part in SampleDependencyThree.Conventions.Exports.Ashlar())
            yield return part;
    }
};
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
