//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes", "eyJsIjp7ImwiOjE0LCJmIjoiSW5wdXQxLmNzIiwibSI6IlJlZ2lzdGVyIn0sImEiOnsiYSI6ZmFsc2UsImkiOmZhbHNlLCJtIjpbIlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zIiwiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMuQWJzdHJhY3Rpb25zIl0sIm5hIjpbXSwiZCI6W119LCJ0Ijp7ImYiOjEsIm5zZiI6W3siZiI6MywibiI6WyJKZXRCcmFpbnMuQW5ub3RhdGlvbnMiXX1dLCJuZiI6W10sImsiOltdLCJ3IjpbeyJpIjpmYWxzZSwiYSI6IlN5c3RlbS5Qcml2YXRlLkNvcmVMaWIiLCJiIjoiU3lzdGVtLkNvbXBvbmVudE1vZGVsLkVkaXRvckJyb3dzYWJsZUF0dHJpYnV0ZSJ9XSwicyI6W10sImF0IjpbeyJpIjpmYWxzZSwiYSI6IlN5c3RlbS5Qcml2YXRlLkNvcmVMaWIiLCJ0IjoiU3lzdGVtLkF0dHJpYnV0ZSJ9XSwidGEiOltdLCJhIjpmYWxzZSwiaSI6ZmFsc2UsIm0iOlsiUm9ja2V0LlN1cmdlcnkuQ29udmVudGlvbnMiLCJSb2NrZXQuU3VyZ2VyeS5Db252ZW50aW9ucy5BYnN0cmFjdGlvbnMiXSwibmEiOltdLCJkIjpbXX19")]
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
                        yield return RocketSurgeryConventionsAbstractions.GetType("EnumPolyfill");
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions);
                        yield return RocketSurgeryConventions.GetType("Microsoft.Extensions.DependencyInjection.LoggingBuilder");
                        yield return typeof(global::Microsoft.Extensions.DependencyInjection.RocketSurgeryServiceCollectionExtensions);
                        yield return typeof(global::Microsoft.Extensions.Logging.RocketSurgeryLoggingExtensions);
                        yield return RocketSurgeryConventionsAbstractions.GetType("RegexPolyfill");
                        yield return typeof(global::Rocket.Surgery.Conventions.AbstractConventionContextBuilderExtensions);
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.Adapters.IServiceFactoryAdapter");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.Adapters.ServiceFactoryAdapter`1");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.AssemblyProviderFactory");
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderApplicationDelegate);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderDelegateResult);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderEnvironmentDelegate);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationOptionsExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilder);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilderExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextExtensions);
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.ConventionContextHelpers");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionDependency");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionHostBuilderExtensions);
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionHostBuilderExtensions+ServiceProviderWrapper`1");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionProvider");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionProviderFactory);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionWithDependencies);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyDirection);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Extensions.RocketSurgerySetupExtensions");
                        yield return typeof(global::Rocket.Surgery.Conventions.HostType);
                        yield return typeof(global::Rocket.Surgery.Conventions.IAssemblyProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionDependency);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionWithDependencies);
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                        yield return typeof(global::Rocket.Surgery.Conventions.IReadOnlyServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.IServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.RocketLoggingOptions);
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.LoggingExtensions");
                        yield return typeof(global::Rocket.Surgery.Conventions.ReadOnlyServiceProviderDictionary);
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.AppDomainAssemblyProvider");
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.AssemblyCandidateResolver");
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.AssemblyCandidateResolver+Dependency");
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.AssemblyProviderAssemblySelector");
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.DefaultAssemblyProvider");
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.DependencyClassification");
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.IAssemblyProviderAssemblySelector);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeFilter);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeProviderAssemblySelector);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeSelector);
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.TypeFilter");
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeKindFilter);
                        yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Reflection.TypeProviderAssemblySelector");
                        yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionaryExtensions);
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ServiceProviderFactoryAdapter");
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Testing.TestConventionContextBuilderExtensions);
                        yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ThrowHelper");
                        yield return RocketSurgeryConventionsAbstractions.GetType("StringPolyfill");
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