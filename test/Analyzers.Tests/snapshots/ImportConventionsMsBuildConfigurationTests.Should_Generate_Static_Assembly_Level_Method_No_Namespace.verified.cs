﻿//HintName: Imported_Assembly_Conventions.cs
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.Namespace", "")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.ClassName", "MyImports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Imports.MethodName", "GetConventions")]
/// <summary>
/// The class defined for importing conventions into this assembly
/// </summary>
[System.Runtime.CompilerServices.CompilerGenerated]
internal static partial class MyImports
{
    /// <summary>
    /// The conventions imported into this assembly
    /// </summary>
    public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
    {
        foreach (var convention in Dep1.Dep1Exports.GetConventions(serviceProvider))
            yield return convention;
        foreach (var convention in Dep2Exports.GetConventions(serviceProvider))
            yield return convention;
        foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
            yield return convention;
    }
}