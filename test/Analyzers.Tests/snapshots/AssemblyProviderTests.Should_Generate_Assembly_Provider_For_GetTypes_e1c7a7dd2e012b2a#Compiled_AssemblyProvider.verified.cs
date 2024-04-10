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
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Polyfill");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("RegexPolyfill");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Adapters.IServiceFactoryAdapter");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Adapters.ServiceFactoryAdapter`1");
                        yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute<>);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.AssemblyProviderFactory");
                        yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderApplicationDelegate);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderDelegateResult);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderEnvironmentDelegate);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationOptionsExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilder);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilderExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionDependency");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionHostBuilderExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionHostBuilderExtensions+ServiceProviderWrapper`1");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionProvider");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionProviderFactory);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionsConfigurationAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionWithDependencies);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyDirection);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependentOfConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependentOfConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependsOnConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependsOnConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.ExportConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ExportConventionsAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ExportedConventionsAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.HostType);
                        yield return typeof(global::Rocket.Surgery.Conventions.IAssemblyProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionDependency);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionWithDependencies);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                        yield return typeof(global::Rocket.Surgery.Conventions.ImportConventionsAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.IReadOnlyServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.IServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.LiveConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.RocketLoggingOptions);
                        yield return typeof(global::Rocket.Surgery.Conventions.ReadOnlyServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionaryExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ServiceProviderFactoryAdapter");
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ThrowHelper");
                        yield return typeof(global::Rocket.Surgery.Conventions.UnitTestConventionAttribute);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("StringPolyfill");
                        break;
                }
            }
        }
    }
}