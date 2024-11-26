//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
#pragma warning disable CA1822
        public ICompiledTypeProvider CreateAssemblyProvider(ConventionContextBuilder builder)
        {
            builder.Set(HostType.UnitTest);
            return new AssemblyProvider();
        }

        [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private class AssemblyProvider() : ICompiledTypeProvider
        {
            IEnumerable<Assembly> ICompiledTypeProvider.GetAssemblies(Action<IReflectionAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                yield break;
            }

            IEnumerable<Type> ICompiledTypeProvider.GetTypes(Func<IReflectionTypeSelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                yield break;
            }
        }
    }
}
