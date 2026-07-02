//HintName: Rocket.Surgery.Clavus.Analyzers/Rocket.Surgery.Clavus.ClavusAttributesGenerator/Exported_Conventions.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Clavus;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ClavusConfigurationData.Exports.Namespace", "Source.Space")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ClavusConfigurationData.Exports.ClassName", "Exports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ClavusConfigurationData.Exports.MethodName", "GetConventions")]
[assembly: ExportedConventions(typeof(Contrib1), typeof(Contrib2), typeof(Contrib3), typeof(Contrib4))]
namespace Source.Space
{
    /// <summary>
    /// The class defined for exporting conventions from this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Clavus.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static partial class Exports
    {
        /// <summary>
        /// The conventions exports from this assembly
        /// </summary>
        public static IEnumerable<IClavusPartMetadata> GetConventions(ClavusContextBuilder builder)
        {
            yield return new ClavusPartMetadata(new Contrib1(), HostType.Undefined, ClavusCategory.Application);
            yield return new ClavusPartMetadata(new Contrib2(), HostType.Undefined, ClavusCategory.Application);
            yield return new ClavusPartMetadata(new Contrib3(), HostType.Undefined, ClavusCategory.Application);
            yield return new ClavusPartMetadata(new Contrib4(), HostType.Undefined, ClavusCategory.Application);
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
