//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#pragma warning disable CA1822
#pragma warning disable CS8618
#pragma warning disable CS8603
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
                    // FilePath: Input0.cs Member: Register
                    case 18:
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationConvention);
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionDependency");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedAsyncConvention<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedConvention<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.Hosting.IHostCreatedAsyncConvention<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.Hosting.IHostCreatedConvention<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionDependency);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionFactory);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionWithDependencies);
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                        break;
                }
            }

            private Assembly _RocketSurgeryConventionsAbstractions;
            private Assembly RocketSurgeryConventionsAbstractions => _RocketSurgeryConventionsAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null"));
        }
    }
}