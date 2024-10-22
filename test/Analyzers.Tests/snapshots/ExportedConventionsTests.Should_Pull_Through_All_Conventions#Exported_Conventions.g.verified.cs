//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Exported_Conventions.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.Namespace", "Source.Space")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.ClassName", "Exports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.MethodName", "GetConventions")]
[assembly: ExportedConventions(typeof(Contrib1), typeof(Contrib2), typeof(Contrib3), typeof(Contrib4))]
[assembly: Convention(typeof(Contrib2))]
namespace Source.Space
{
    /// <summary>
    /// The class defined for exporting conventions from this assembly
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static partial class Exports
    {
        /// <summary>
        /// The conventions exports from this assembly
        /// </summary>
        public static IEnumerable<IConventionMetadata> GetConventions(ConventionContextBuilder builder)
        {
            yield return new ConventionMetadata(new Contrib1(), HostType.Undefined, ConventionCategory.Application);
            yield return new ConventionMetadata(new Contrib2(), HostType.Undefined, ConventionCategory.Application);
            yield return new ConventionMetadata(new Contrib3(), HostType.Undefined, ConventionCategory.Application);
            yield return new ConventionMetadata(new Contrib4(), HostType.Undefined, ConventionCategory.Application);
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
