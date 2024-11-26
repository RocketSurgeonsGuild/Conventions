//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using System.Runtime.Loader;

[assembly: Rocket.Surgery.Conventions.AssemblyProviderAttribute(typeof(TestProject.Conventions.AssemblyProvider))]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public ICompiledTypeProvider CreateAssemblyProvider() => new AssemblyProvider(builder.Properties.GetRequiredService<AssemblyLoadContext>());
    }

    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    file class AssemblyProvider(AssemblyLoadContext context) : ICompiledTypeProvider
    {
        IEnumerable<Assembly> ICompiledTypeProvider.GetAssemblies(Action<IReflectionAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
        {
            yield break;
        }

        IEnumerable<Type> ICompiledTypeProvider.GetTypes(Func<IReflectionTypeSelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
        {
            switch (lineNumber)
            {
                // FilePath: Input0.cs Expression: kNmlrwWR/6Jy3Txc1PYKDQ==
                case 18:
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.<ICompiledTypeProvider>F742B48AECD3090FCED73F7295C987403C958108A026334C95957DD29503853D0__AP");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionCategory+ValueEqualityComparer");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionDependency");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionHostBuilderExtensions+ServiceProviderWrapper`1");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionProvider");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ServiceProviderFactoryAdapter");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ThrowHelper");
                    break;
            }
        }

        private Assembly _RocketSurgeryConventionsAbstractions;
        private Assembly RocketSurgeryConventionsAbstractions => _RocketSurgeryConventionsAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null"));
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
