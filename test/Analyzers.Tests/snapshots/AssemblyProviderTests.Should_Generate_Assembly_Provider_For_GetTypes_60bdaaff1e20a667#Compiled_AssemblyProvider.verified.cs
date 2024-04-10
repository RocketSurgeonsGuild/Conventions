//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace TestProject.Conventions
{
    internal static partial class Imports
    {
        private class AssemblyProvider : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                yield break;
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 9:
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("EnumPolyfill");
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Microsoft.Extensions.DependencyInjection.LoggingBuilder");
                        yield return typeof(global::Microsoft.Extensions.DependencyInjection.RocketSurgeryServiceCollectionExtensions);
                        yield return typeof(global::Microsoft.Extensions.Logging.RocketSurgeryLoggingExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Polyfill");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("RegexPolyfill");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Adapters.IServiceFactoryAdapter");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Adapters.ServiceFactoryAdapter`1");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.AssemblyProviderFactory");
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderApplicationDelegate);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderDelegateResult);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderEnvironmentDelegate);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationOptionsExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilder);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilderExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilderExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionContextHelpers");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionDependency");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionHostBuilderExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionHostBuilderExtensions+ServiceProviderWrapper`1");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionProvider");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionProviderFactory);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionWithDependencies);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyDirection);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Extensions.RocketSurgerySetupExtensions");
                        yield return typeof(global::Rocket.Surgery.Conventions.HostType);
                        yield return typeof(global::Rocket.Surgery.Conventions.IAssemblyProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionDependency);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionWithDependencies);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                        yield return typeof(global::Rocket.Surgery.Conventions.IReadOnlyServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.IServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.RocketLoggingOptions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.LoggingExtensions");
                        yield return typeof(global::Rocket.Surgery.Conventions.ReadOnlyServiceProviderDictionary);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AppDomainAssemblyProvider");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AssemblyCandidateResolver");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AssemblyCandidateResolver+Dependency");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AssemblyProviderAssemblySelector");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.DefaultAssemblyProvider");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.DependencyClassification");
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.IAssemblyProviderAssemblySelector);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeFilter);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeProviderAssemblySelector);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeSelector);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.TypeFilter");
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeKindFilter);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.TypeProviderAssemblySelector");
                        yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionaryExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ServiceProviderFactoryAdapter");
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Testing.TestConventionContextBuilderExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ThrowHelper");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("StringPolyfill");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("System.Runtime.CompilerServices.IsExternalInit");
                        break;
                }
            }
        }
    }
}