//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Runtime.Loader;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
[assembly: Rocket.Surgery.Conventions.AssemblyProviderAttribute(typeof(TestProject.Conventions.AssemblyProvider))]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public IAssemblyProvider CreateAssemblyProvider() => new AssemblyProvider(builder.Properties.GetRequiredService<AssemblyLoadContext>());
    }

    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    file class AssemblyProvider(AssemblyLoadContext context) : IAssemblyProvider
    {
        IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
        {
            yield break;
        }

        IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
        {
            switch (lineNumber)
            {
                // FilePath: Input0.cs Expression: 5delP+bD11tpFgMXLuGBUA==
                case 18:
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderApplicationDelegate);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderEnvironmentDelegate);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionCategory);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionCategory+ValueEqualityComparer");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionDependency");
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionMetadata);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyDirection);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedAsyncConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.HostType);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeInfoFilter);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeKindFilter);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ServiceProviderFactoryAdapter");
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                    break;
            }
        }

        private Assembly _RocketSurgeryConventionsAbstractions;
        private Assembly RocketSurgeryConventionsAbstractions => _RocketSurgeryConventionsAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null"));
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
