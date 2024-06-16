﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
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
                    // FilePath: Input0.cs Expression: VwNfV/3uzT3foomIq+8KzA==
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
#pragma warning restore CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
#nullable restore
