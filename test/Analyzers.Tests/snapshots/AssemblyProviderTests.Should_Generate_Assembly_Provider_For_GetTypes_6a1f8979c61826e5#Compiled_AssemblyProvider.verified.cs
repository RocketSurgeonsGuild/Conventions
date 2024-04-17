//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Runtime.Loader;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
#pragma warning disable CA1822
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
                    // FilePath: Input1.cs Member: Register
                    case 18:
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.AppDomainAssemblyProvider");
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.AppDomainConventionFactory);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.AssemblyConventionFactory);
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.AssemblyProviderAssemblySelector");
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.DefaultAssemblyProvider");
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.TypeFilter");
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.TypeProviderAssemblySelector");
                        break;
                }
            }

            private Assembly _RocketSurgeryConventions;
            private Assembly RocketSurgeryConventions => _RocketSurgeryConventions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions, Version=version, Culture=neutral, PublicKeyToken=null"));
        }
    }
}