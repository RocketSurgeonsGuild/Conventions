//HintName: Exported_Conventions.cs
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.Namespace", "Source.Space")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.ClassName", "Exports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.MethodName", "GetConventions")]
[assembly: ExportedConventions(typeof(Contrib1), typeof(Contrib3), typeof(Contrib4), typeof(Contrib2))]
[assembly: Convention(typeof(Contrib2))]
namespace Source.Space
{
    /// <summary>
    /// The class defined for exporting conventions from this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
        /// <summary>
        /// The conventions exports from this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(new Contrib1(), HostType.Undefined);
            yield return new ConventionWithDependencies(new Contrib3(), HostType.Undefined);
            yield return new ConventionWithDependencies(new Contrib4(), HostType.Undefined);
            yield return new ConventionWithDependencies(new Contrib2(), HostType.Undefined);
        }
    }
}