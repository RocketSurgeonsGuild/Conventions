//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Exported_Conventions.g.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.Namespace", "TestProject.Conventions")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.ClassName", "Exports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.MethodName", "GetConventions")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib))]
namespace TestProject.Conventions
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
            yield return new ConventionMetadata(new Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib(builder.Properties.GetService<Rocket.Surgery.Conventions.Tests.IService>(), builder.Properties.GetService<Rocket.Surgery.Conventions.Tests.IServiceB>(), builder.Properties.GetService<Rocket.Surgery.Conventions.Tests.IServiceC>()), HostType.Undefined);
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
#nullable restore
