//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Runtime.Loader;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AssemblyProvider(builder.Properties.GetRequiredService<AssemblyLoadContext>());
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private class AssemblyProvider(AssemblyLoadContext context) : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
            {
                yield break;
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
            {
                switch (lineNumber)
                {
                    // FilePath: Input1.cs Expression: wgnEQ0+p18R+bMgUsWT3sw==
                    case 14:
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.LibraryIntegrationTests+Mapper");
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.LibraryIntegrationTests+Profile1");
                        break;
                    // FilePath: Input1.cs Expression: l3hPOV3IoAdlDDyp5Ce76w==
                    case 15:
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.LibraryIntegrationTests+A");
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.LibraryIntegrationTests+C");
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.LibraryIntegrationTests+D");
                        yield return typeof(global::Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.LibraryIntegrationTests.DocumentCreatedByValueResolver<, >);
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.LibraryIntegrationTests+E");
                        break;
                }
            }

            private Assembly _RocketSurgeryConventionsAnalyzersTests;
            private Assembly RocketSurgeryConventionsAnalyzersTests => _RocketSurgeryConventionsAnalyzersTests ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Analyzers.Tests, Version=version, Culture=neutral, PublicKeyToken=null"));
        }
    }
}
#pragma warning restore CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
#nullable restore
