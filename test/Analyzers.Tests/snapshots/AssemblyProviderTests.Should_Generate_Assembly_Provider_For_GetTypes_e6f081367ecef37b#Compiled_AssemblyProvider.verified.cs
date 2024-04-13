//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes", "eyJsIjp7ImwiOjE2LCJmIjoiSW5wdXQxLmNzIiwibSI6IlJlZ2lzdGVyIn0sImEiOnsiYSI6dHJ1ZSwiaSI6ZmFsc2UsIm0iOltdLCJuYSI6W10sImQiOltdfSwidCI6eyJmIjoxLCJuc2YiOltdLCJuZiI6W10sInRrIjpbXSwidGkiOltdLCJ3IjpbXSwicyI6W10sImF0IjpbXSwidGEiOlt7ImkiOnRydWUsInQiOlt7ImEiOiJSb2NrZXQuU3VyZ2VyeS5Db252ZW50aW9ucy5BYnN0cmFjdGlvbnMiLCJ0IjoiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMuQ29uZmlndXJhdGlvbi5JQ29uZmlndXJhdGlvbkFzeW5jQ29udmVudGlvbiJ9LHsiYSI6IlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zLkFic3RyYWN0aW9ucyIsInQiOiJSb2NrZXQuU3VyZ2VyeS5Db252ZW50aW9ucy5Db25maWd1cmF0aW9uLklDb25maWd1cmF0aW9uQ29udmVudGlvbiJ9LHsiYSI6IlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zLkFic3RyYWN0aW9ucyIsInQiOiJSb2NrZXQuU3VyZ2VyeS5Db252ZW50aW9ucy5EZXBlbmRlbmN5SW5qZWN0aW9uLklTZXJ2aWNlQXN5bmNDb252ZW50aW9uIn0seyJhIjoiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMuQWJzdHJhY3Rpb25zIiwidCI6IlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zLkRlcGVuZGVuY3lJbmplY3Rpb24uSVNlcnZpY2VDb252ZW50aW9uIn1dfV0sImEiOnRydWUsImkiOmZhbHNlLCJtIjpbXSwibmEiOltdLCJkIjpbXX19")]
namespace TestProject.Conventions
{
    internal partial class Imports
    {
        public IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AssemblyProvider();
        private class AssemblyProvider() : IAssemblyProvider
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
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention);
                        yield return typeof(global::TestConvention);
                        break;
                }
            }
        }
    }
}