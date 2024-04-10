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
                    case 6:
                        switch (filePath)
                        {
                            case "Input1.cs":
                                yield return typeof(global::TestConvention).Assembly;
                                break;
                            case "Input2.cs":
                                yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationDebugViewContext).Assembly;
                                yield return typeof(global::Microsoft.Extensions.DependencyInjection.ActivatorUtilities).Assembly;
                                yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions).Assembly;
                                yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute).Assembly;
                                yield return typeof(global::Dep1.Dep1Exports).Assembly;
                                yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                                yield return typeof(global::Dep2Exports).Assembly;
                                yield return typeof(global::System.IServiceProvider).Assembly;
                                yield return typeof(global::Internal.Console).Assembly;
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