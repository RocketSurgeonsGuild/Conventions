﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#pragma warning disable CA1822
#pragma warning disable CS8618
#pragma warning disable CS8603
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
                    // FilePath: Input0.cs Member: Register
                    case 18:
                        yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute<>);
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
}