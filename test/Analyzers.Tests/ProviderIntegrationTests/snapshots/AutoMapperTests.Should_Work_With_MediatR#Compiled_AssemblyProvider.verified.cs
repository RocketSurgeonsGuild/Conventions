﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Runtime.Loader;

namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
#pragma warning disable CA1822
        public IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AssemblyProvider(builder.Properties.GetRequiredService<AssemblyLoadContext>());
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private class AssemblyProvider(AssemblyLoadContext context) : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                yield break;
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    // FilePath: Input1.cs Member: Register
                    case 9:
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.AutoMapperTests+Mapper");
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.AutoMapperTests+Profile1");
                        break;
                    // FilePath: Input1.cs Member: Register
                    case 10:
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.AutoMapperTests+A");
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.AutoMapperTests+B");
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.AutoMapperTests+C");
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.AutoMapperTests+D");
                        yield return RocketSurgeryConventionsAnalyzersTests.GetType("Rocket.Surgery.Conventions.Analyzers.Tests.ProviderIntegrationTests.AutoMapperTests+E");
                        break;
                }
            }

            private Assembly _RocketSurgeryConventionsAnalyzersTests;
            private Assembly RocketSurgeryConventionsAnalyzersTests => _RocketSurgeryConventionsAnalyzersTests ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Analyzers.Tests, Version=version, Culture=neutral, PublicKeyToken=null"));
        }
    }
}