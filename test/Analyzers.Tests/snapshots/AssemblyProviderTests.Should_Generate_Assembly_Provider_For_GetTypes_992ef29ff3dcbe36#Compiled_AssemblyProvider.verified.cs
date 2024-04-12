//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes", "eyJsIjp7ImwiOjE0LCJmIjoiSW5wdXQxLmNzIiwibSI6IlJlZ2lzdGVyIn0sImEiOnsiYSI6ZmFsc2UsImkiOmZhbHNlLCJtIjpbIlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zIiwiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMuQWJzdHJhY3Rpb25zIl0sIm5hIjpbXSwiZCI6W119LCJ0Ijp7ImYiOjEsIm5zZiI6W10sIm5mIjpbXSwidGsiOltdLCJ0aSI6W10sInciOlt7ImkiOnRydWUsImEiOiJTeXN0ZW0uUHJpdmF0ZS5Db3JlTGliIiwiYiI6IlN5c3RlbS5Db21wb25lbnRNb2RlbC5FZGl0b3JCcm93c2FibGVBdHRyaWJ1dGUifV0sInMiOltdLCJhdCI6W10sInRhIjpbXSwiYSI6ZmFsc2UsImkiOmZhbHNlLCJtIjpbIlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zIiwiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMuQWJzdHJhY3Rpb25zIl0sIm5hIjpbXSwiZCI6W119fQ==")]
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
                        yield return RocketSurgeryConventionsAbstractions.GetType("Polyfill");
                        yield return RocketSurgeryConventions.GetType("System.Runtime.CompilerServices.IsExternalInit");
                        break;
                }
            }

            private Assembly _RocketSurgeryConventions;
            private Assembly RocketSurgeryConventions => _RocketSurgeryConventions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions, Version=version, Culture=neutral, PublicKeyToken=null"));

            private Assembly _RocketSurgeryConventionsAbstractions;
            private Assembly RocketSurgeryConventionsAbstractions => _RocketSurgeryConventionsAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null"));
        }
    }
}