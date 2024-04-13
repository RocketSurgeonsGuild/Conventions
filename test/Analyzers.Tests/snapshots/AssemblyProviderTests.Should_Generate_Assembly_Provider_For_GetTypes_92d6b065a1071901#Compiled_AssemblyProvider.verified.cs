//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes", "eyJsIjp7ImwiOjE2LCJmIjoiSW5wdXQxLmNzIiwibSI6IlJlZ2lzdGVyIn0sImEiOnsiYSI6dHJ1ZSwiaSI6ZmFsc2UsIm0iOltdLCJuYSI6W10sImQiOltdfSwidCI6eyJmIjoxLCJuc2YiOlt7ImYiOjIsIm4iOlsiTWljcm9zb2Z0LkV4dGVuc2lvbnMuQ29uZmlndXJhdGlvbiJdfV0sIm5mIjpbXSwidGsiOltdLCJ0aSI6W10sInciOltdLCJzIjpbXSwiYXQiOltdLCJ0YSI6W10sImEiOnRydWUsImkiOmZhbHNlLCJtIjpbXSwibmEiOltdLCJkIjpbXX19")]
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
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationDebugViewContext);
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationExtensions);
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationKeyNameAttribute);
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationPath);
                        yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationRootExtensions);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfiguration);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationBuilder);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationManager);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationProvider);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationRoot);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationSection);
                        yield return typeof(global::Microsoft.Extensions.Configuration.IConfigurationSource);
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions);
                        break;
                }
            }
        }
    }
}