//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace TestProject.Conventions
{
    internal static partial class Imports
    {
        private class AssemblyProvider : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 7:
                        switch (filePath)
                        {
                            case "Input1.cs":
                                yield return typeof(global::TestConvention2).Assembly;
                                break;
                            case "Input2.cs":
                                yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationRootExtensions).Assembly;
                                yield return typeof(global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions).Assembly;
                                yield return typeof(global::Rocket.Surgery.Conventions.Testing.TestConventionContextBuilderExtensions).Assembly;
                                yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention).Assembly;
                                yield return typeof(global::Sample.DependencyOne.Class1).Assembly;
                                yield return typeof(global::SampleDependencyThree.Conventions.Exports).Assembly;
                                yield return typeof(global::Sample.DependencyTwo.Class2).Assembly;
                                yield return typeof(global::System.ComponentModel.CancelEventArgs).Assembly;
                                yield return typeof(global::System.Void).Assembly;
                                yield return typeof(global::TestConvention2).Assembly;
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
