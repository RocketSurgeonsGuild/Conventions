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
                // FilePath: Input0.cs Expression: J0kQ4rZYNH6Zh2R2FNj0/Q==
                case 18:
                    yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionCategory);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionCategoryAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilder);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionHostBuilderExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionMetadata);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionProvider");
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionsConfigurationAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependentOfConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependentOfConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependsOnConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependsOnConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.ExportConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ExportConventionsAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ExportedConventionsAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ImportConventionsAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.LiveConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.UnitTestConventionAttribute);
                    break;
            }
        }

        private Assembly _RocketSurgeryConventionsAbstractions;
        private Assembly RocketSurgeryConventionsAbstractions => _RocketSurgeryConventionsAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null"));
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
