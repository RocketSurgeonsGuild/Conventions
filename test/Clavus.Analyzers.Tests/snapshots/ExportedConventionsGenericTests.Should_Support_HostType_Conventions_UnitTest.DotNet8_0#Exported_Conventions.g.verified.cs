//HintName: Clavus.Analyzers/Clavus.ClavusAttributesGenerator/Exported_Conventions.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Clavus;
using Clavus.Infrastructure;

[assembly: System.Reflection.AssemblyMetadata("ClavusConfigurationData.Exports.Namespace", "TestProject.Conventions")]
[assembly: System.Reflection.AssemblyMetadata("ClavusConfigurationData.Exports.ClassName", "Exports")]
[assembly: System.Reflection.AssemblyMetadata("ClavusConfigurationData.Exports.MethodName", "GetConventions")]
[assembly: ExportedConventions(typeof(Clavus.Tests.Contrib))]
namespace TestProject.Conventions
{
    /// <summary>
    /// The class defined for exporting conventions from this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Clavus.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static partial class Exports
    {
        /// <summary>
        /// The conventions exports from this assembly
        /// </summary>
        public static IEnumerable<IClavusPartMetadata> GetConventions(ClavusContextBuilder builder)
        {
            yield return new ClavusPartMetadata(new Clavus.Tests.Contrib(), HostType.UnitTest, ClavusCategory.Application);
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
