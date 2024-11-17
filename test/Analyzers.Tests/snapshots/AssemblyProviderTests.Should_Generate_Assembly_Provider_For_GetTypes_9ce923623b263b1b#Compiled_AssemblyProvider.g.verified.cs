//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
[assembly: Rocket.Surgery.Conventions.AssemblyProviderAttribute(typeof(TestProject.Conventions.AssemblyProvider))]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public ICompiledTypeProvider CreateAssemblyProvider() => new AssemblyProvider();
    }

    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    file class AssemblyProvider() : ICompiledTypeProvider
    {
        IEnumerable<Assembly> ICompiledTypeProvider.GetAssemblies(Action<IReflectionAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
        {
            yield break;
        }

        IEnumerable<Type> ICompiledTypeProvider.GetTypes(Func<IReflectionTypeSelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
        {
            switch (lineNumber)
            {
                // FilePath: Input0.cs Expression: hWvVhQgDipowXuL6LTZuqA==
                case 18:
                    yield return typeof(global::Rocket.Surgery.Conventions.AssemblyProviderExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderApplicationDelegate);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderDelegateResult);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderEnvironmentDelegate);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationOptionsExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionCategory);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilder);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionHostBuilderExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionMetadata);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyDirection);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedAsyncConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.IHostCreatedAsyncConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.IHostCreatedConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.HostType);
                    yield return typeof(global::Rocket.Surgery.Conventions.ICompiledTypeProvider);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionContext);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionDependency);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionFactory);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionMetadata);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionProvider);
                    yield return typeof(global::Rocket.Surgery.Conventions.ImportHelpers);
                    yield return typeof(global::Rocket.Surgery.Conventions.IReadOnlyServiceProviderDictionary);
                    yield return typeof(global::Rocket.Surgery.Conventions.IServiceProviderDictionary);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.RocketLoggingOptions);
                    yield return typeof(global::Rocket.Surgery.Conventions.ReadOnlyServiceProviderDictionary);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.IReflectionAssemblySelector);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeFilter);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.IReflectionTypeSelector);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeSelector);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeInfoFilter);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeKindFilter);
                    yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionary);
                    yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionaryExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                    break;
            }
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
