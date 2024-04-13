//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace TestProject.Conventions
{
    internal static partial class Imports
    {
        private class AssemblyProvider(AssemblyLoadContext context) : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 202:
                        yield return typeof(global::TestConvention).Assembly;
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions).Assembly;
                        yield return typeof(global::Dep1.Dep1Exports).Assembly;
                        yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                        yield return typeof(global::Dep2Exports).Assembly;
                        yield return context.LoadFromAssemblyName(TestProjectVersion0000CultureneutralPublicKeyTokennull);
                        break;
                }
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 14:
                        break;
                }
            }

            private static AssemblyName _TestProjectVersion0000CultureneutralPublicKeyTokennull;
            private static AssemblyName TestProjectVersion0000CultureneutralPublicKeyTokennull => _TestProjectVersion0000CultureneutralPublicKeyTokennull ??= new AssemblyName("TestProject, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        }
    }
}