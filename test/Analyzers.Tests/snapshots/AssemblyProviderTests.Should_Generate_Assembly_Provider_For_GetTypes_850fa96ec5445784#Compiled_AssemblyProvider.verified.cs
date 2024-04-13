//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Runtime.Loader;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes", "eyJsIjp7ImwiOjE2LCJmIjoiSW5wdXQxLmNzIiwibSI6IlJlZ2lzdGVyIn0sImEiOnsiYSI6ZmFsc2UsImkiOmZhbHNlLCJtIjpbIlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zLkFic3RyYWN0aW9ucyJdLCJuYSI6W10sImQiOltdfSwidCI6eyJmIjoxLCJuc2YiOlt7ImYiOjMsIm4iOlsiSmV0QnJhaW5zLkFubm90YXRpb25zIl19XSwibmYiOltdLCJ0ayI6W10sInRpIjpbeyJmIjpmYWxzZSwidCI6WzJdfV0sInciOltdLCJzIjpbXSwiYXQiOlt7ImkiOmZhbHNlLCJhIjoiU3lzdGVtLlByaXZhdGUuQ29yZUxpYiIsInQiOiJTeXN0ZW0uQXR0cmlidXRlIn1dLCJ0YSI6W10sImEiOmZhbHNlLCJpIjpmYWxzZSwibSI6WyJSb2NrZXQuU3VyZ2VyeS5Db252ZW50aW9ucy5BYnN0cmFjdGlvbnMiXSwibmEiOltdLCJkIjpbXX19")]
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
                        yield return RocketSurgeryConventionsAbstractions.GetType("EnumPolyfill");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Polyfill");
                        yield return RocketSurgeryConventionsAbstractions.GetType("RegexPolyfill");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.Abstractions.Conventions.Imports");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.Abstractions.Conventions.Imports+AssemblyProvider");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.Adapters.IServiceFactoryAdapter");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.Adapters.ServiceFactoryAdapter`1");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.AssemblyProviderFactory");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionDependency");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionProvider");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ServiceProviderFactoryAdapter");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ThrowHelper");
                        yield return RocketSurgeryConventionsAbstractions.GetType("StringPolyfill");
                        break;
                }
            }

            private Assembly _RocketSurgeryConventionsAbstractions;
            private Assembly RocketSurgeryConventionsAbstractions => _RocketSurgeryConventionsAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null"));
        }
    }
}