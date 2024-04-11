//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes", "eyJsIjp7ImwiOjE0LCJmIjoiSW5wdXQxLmNzIiwibSI6IlJlZ2lzdGVyIn0sImEiOnsiYSI6ZmFsc2UsImkiOmZhbHNlLCJtIjpbIlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zIiwiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMuQWJzdHJhY3Rpb25zIl0sIm5hIjpbXSwiZCI6W119LCJ0Ijp7ImYiOjEsIm5zZiI6W10sIm5mIjpbXSwiayI6W10sInciOltdLCJzIjpbeyJpIjp0cnVlLCJiIjoiU3lzdGVtLkRpYWdub3N0aWNzLkNvZGVBbmFseXNpcy5SZXF1aXJlc1VucmVmZXJlbmNlZENvZGVBdHRyaWJ1dGUifV0sImF0IjpbXSwidGEiOltdLCJhIjpmYWxzZSwiaSI6ZmFsc2UsIm0iOlsiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMiLCJSb2NrZXQuU3VyZ2VyeS5Db252ZW50aW9ucy5BYnN0cmFjdGlvbnMiXSwibmEiOltdLCJkIjpbXX19")]
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