//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Runtime.Loader;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
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
                    case 18:
                        yield return SampleDependencyOne.GetType("Sample.DependencyOne.Example1+Validator");
                        yield return SampleDependencyThree.GetType("Sample.DependencyThree.Example3+Validator");
                        yield return SampleDependencyTwo.GetType("Sample.DependencyTwo.Example2+Validator");
                        break;
                }
            }

            private Assembly _SampleDependencyOne;
            private Assembly SampleDependencyOne => _SampleDependencyOne ??= context.LoadFromAssemblyName(new AssemblyName("SampleDependencyOne, Version=version, Culture=neutral, PublicKeyToken=null"));

            private Assembly _SampleDependencyThree;
            private Assembly SampleDependencyThree => _SampleDependencyThree ??= context.LoadFromAssemblyName(new AssemblyName("SampleDependencyThree, Version=version, Culture=neutral, PublicKeyToken=null"));

            private Assembly _SampleDependencyTwo;
            private Assembly SampleDependencyTwo => _SampleDependencyTwo ??= context.LoadFromAssemblyName(new AssemblyName("SampleDependencyTwo, Version=version, Culture=neutral, PublicKeyToken=null"));
        }
    }
}