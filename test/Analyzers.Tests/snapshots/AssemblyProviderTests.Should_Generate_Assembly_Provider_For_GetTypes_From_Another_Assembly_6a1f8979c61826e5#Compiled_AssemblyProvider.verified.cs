//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Runtime.Loader;

namespace TestProject.Conventions
{
    internal partial class Imports
    {
        public IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AssemblyProvider(builder.Properties.GetRequiredService<AssemblyLoadContext>());
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
                    case 16:
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.AppDomainAssemblyProvider");
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