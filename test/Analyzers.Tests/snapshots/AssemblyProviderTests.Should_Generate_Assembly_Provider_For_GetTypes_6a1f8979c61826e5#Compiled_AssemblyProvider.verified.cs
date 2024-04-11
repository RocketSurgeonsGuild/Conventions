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
                yield break;
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 14:
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AppDomainAssemblyProvider");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AssemblyProviderAssemblySelector");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.DefaultAssemblyProvider");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.TypeFilter");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.TypeProviderAssemblySelector");
                        break;
                }
            }

            private static AssemblyName _RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull;
            private static AssemblyName RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull => _RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull ??= new AssemblyName("Rocket.Surgery.Conventions, Version=version, Culture=neutral, PublicKeyToken=null");
        }
    }
}