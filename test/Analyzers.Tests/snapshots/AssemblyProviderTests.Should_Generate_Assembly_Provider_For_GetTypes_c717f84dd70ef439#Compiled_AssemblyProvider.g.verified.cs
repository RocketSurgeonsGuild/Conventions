﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
[assembly: Rocket.Surgery.Conventions.AssemblyProviderAttribute(typeof(TestProject.Conventions.AssemblyProvider))]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public ICompiledTypeProvider CreateAssemblyProvider() => new AssemblyProvider();
    }

    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    file class AssemblyProvider() : ICompiledTypeProvider
    {
        IEnumerable<Assembly> ICompiledTypeProvider.GetAssemblies(Action<IReflectionAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
        {
            yield break;
        }

        IEnumerable<Type> ICompiledTypeProvider.GetTypes(Func<IReflectionTypeSelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
        {
            switch (lineNumber)
            {
                // FilePath: Input0.cs Expression: 4COsKx4JKl1olkioTt6jtg==
                case 18:
                    yield return typeof(global::Rocket.Surgery.Conventions.ImportHelpers);
                    yield return typeof(global::Rocket.Surgery.Conventions.ImportsTypeAttribute);
                    break;
            }
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
