//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Exported_Conventions.cs
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
        public static IEnumerable<IConventionWithDependencies> GetConventions(ConventionContextBuilder builder)
        {
            yield return new ConventionWithDependencies(new Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib(builder.GetService<Rocket.Surgery.Conventions.Tests.IService>(), builder.GetService<Rocket.Surgery.Conventions.Tests.IServiceB>(), builder.GetService<Rocket.Surgery.Conventions.Tests.IServiceC>()), HostType.Undefined);
        }
    }
}