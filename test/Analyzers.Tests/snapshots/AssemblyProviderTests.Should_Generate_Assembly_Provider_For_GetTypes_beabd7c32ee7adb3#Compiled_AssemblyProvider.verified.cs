//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes", "eyJsIjp7ImwiOjE2LCJmIjoiSW5wdXQxLmNzIiwibSI6IlJlZ2lzdGVyIn0sImEiOnsiYSI6ZmFsc2UsImkiOmZhbHNlLCJtIjpbIlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zLkFic3RyYWN0aW9ucyJdLCJuYSI6W10sImQiOltdfSwidCI6eyJmIjoxLCJuc2YiOltdLCJuZiI6W3siZiI6MiwibiI6WyJDb252ZW50aW9uIl19XSwidGsiOlt7ImYiOnRydWUsInQiOls1XX1dLCJ0aSI6W10sInciOltdLCJzIjpbXSwiYXQiOltdLCJ0YSI6W10sImEiOmZhbHNlLCJpIjpmYWxzZSwibSI6WyJSb2NrZXQuU3VyZ2VyeS5Db252ZW50aW9ucy5BYnN0cmFjdGlvbnMiXSwibmEiOltdLCJkIjpbXX19")]
namespace TestProject.Conventions
{
    internal partial class Imports
    {
        private class AssemblyProvider() : IAssemblyProvider
        {
            public IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AssemblyProvider();
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                yield break;
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                yield break;
            }
        }
    }
}