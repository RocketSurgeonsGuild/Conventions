﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

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
                // FilePath: Input0.cs Expression: OPoLUH8GfawO7mDCPPomow==
                case 18:
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedAsyncConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                    break;
            }
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
