//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
#pragma warning disable CA1822
#pragma warning disable CS8618
#pragma warning disable CS8603
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies","")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies","")]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AssemblyProvider();
        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private class AssemblyProvider() : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    // FilePath: Input1.cs Member: Register
                    case 7:
                        switch (filePath)
                        {
                            // FilePath: Input1.cs Member: Register
                            case "Input1.cs":
                                yield return typeof(global::TestConvention).Assembly;
                                break;
                            // FilePath: Input2.cs Member: Register
                            case "Input2.cs":
                                yield return typeof(global::FluentValidation.AbstractValidator<>).Assembly;
                                yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationDebugViewContext).Assembly;
                                yield return typeof(global::Microsoft.Extensions.DependencyInjection.ActivatorUtilities).Assembly;
                                yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions).Assembly;
                                yield return typeof(global::Rocket.Surgery.Conventions.AbstractConventionContextBuilderExtensions).Assembly;
                                yield return typeof(global::Dep1.Dep1Exports).Assembly;
                                yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                                yield return typeof(global::Dep2Exports).Assembly;
                                yield return typeof(global::System.IServiceProvider).Assembly;
                                yield return typeof(global::TestConvention).Assembly;
                                break;
                        }

                        break;
                }
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                yield break;
            }
        }
    }
}