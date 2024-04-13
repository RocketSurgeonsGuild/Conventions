//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies", "eyJsIjp7ImwiOjcsImYiOiJJbnB1dDEuY3MiLCJtIjoiUmVnaXN0ZXIifSwiYSI6eyJhIjpmYWxzZSwiaSI6ZmFsc2UsIm0iOltdLCJuYSI6W10sImQiOlsiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMuQWJzdHJhY3Rpb25zIl19fQ==")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies", "eyJsIjp7ImwiOjgsImYiOiJJbnB1dDEuY3MiLCJtIjoiUmVnaXN0ZXIifSwiYSI6eyJhIjpmYWxzZSwiaSI6ZmFsc2UsIm0iOltdLCJuYSI6W10sImQiOlsiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMuQWJzdHJhY3Rpb25zIl19fQ==")]
namespace TestProject.Conventions
{
    internal partial class Imports
    {
        private class AssemblyProvider() : IAssemblyProvider
        {
            public IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AssemblyProvider();
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    // FilePath: Input1.cs Member: Register
                    case 7:
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions).Assembly;
                        yield return typeof(global::Dep1.Dep1Exports).Assembly;
                        yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                        yield return typeof(global::Dep2Exports).Assembly;
                        yield return typeof(global::TestConvention).Assembly;
                        break;
                    // FilePath: Input1.cs Member: Register
                    case 8:
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions).Assembly;
                        yield return typeof(global::Dep1.Dep1Exports).Assembly;
                        yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                        yield return typeof(global::Dep2Exports).Assembly;
                        yield return typeof(global::TestConvention).Assembly;
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