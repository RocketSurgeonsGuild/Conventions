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

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AssemblyProvider();
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private class AssemblyProvider() : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
            {
                yield break;
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
            {
                switch (lineNumber)
                {
                    // FilePath: Input0.cs Expression: CG3zFudzbyWmHWkmSNc3yQ==
                    case 18:
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationDebugViewContext);
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationExtensions);
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationKeyNameAttribute);
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationPath);
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationRootExtensions);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfiguration);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationBuilder);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationManager);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationProvider);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationRoot);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationSection);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationSource);
                        break;
                }
            }
        }
    }
}