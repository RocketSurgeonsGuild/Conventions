﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using System.Runtime.Loader;

[assembly: Rocket.Surgery.Conventions.AssemblyProviderAttribute(typeof(TestProject.Conventions.AssemblyProvider))]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public ICompiledTypeProvider CreateAssemblyProvider() => new AssemblyProvider(builder.Properties.GetRequiredService<AssemblyLoadContext>());
    }

    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    file class AssemblyProvider(AssemblyLoadContext context) : ICompiledTypeProvider
    {
        IEnumerable<Assembly> ICompiledTypeProvider.GetAssemblies(Action<IReflectionAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
        {
            yield break;
        }

        IEnumerable<Type> ICompiledTypeProvider.GetTypes(Func<IReflectionTypeSelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
        {
            switch (lineNumber)
            {
                // FilePath: Input0.cs Expression: fI1FR/JInsgcwMPg348q7Q==
                case 18:
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.<ICompiledTypeProvider>F742B48AECD3090FCED73F7295C987403C958108A026334C95957DD29503853D0__AP");
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
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionCategory+ValueEqualityComparer");
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContext);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilder);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilderExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextExtensions);
                    yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.ConventionContextHelpers");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionDependency");
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionHostBuilderExtensions);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionHostBuilderExtensions+ServiceProviderWrapper`1");
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionMetadata);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionProvider");
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyDirection);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                    yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Extensions.RocketSurgeryHostCreatedExtensions");
                    yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.Extensions.RocketSurgerySetupExtensions");
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
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                    yield return typeof(global::Rocket.Surgery.Conventions.IReadOnlyServiceProviderDictionary);
                    yield return typeof(global::Rocket.Surgery.Conventions.IServiceProviderDictionary);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.RocketLoggingOptions);
                    yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.LoggingBuilder");
                    yield return RocketSurgeryConventions.GetType("Rocket.Surgery.Conventions.LoggingExtensions");
                    yield return typeof(global::Rocket.Surgery.Conventions.ReadOnlyServiceProviderDictionary);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.IReflectionAssemblySelector);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeFilter);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.IReflectionTypeSelector);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeSelector);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeInfoFilter);
                    yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeKindFilter);
                    yield return typeof(global::Rocket.Surgery.Conventions.RocketSurgeryConfigurationExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.RocketSurgeryLoggingExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.RocketSurgeryServiceCollectionExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionary);
                    yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionaryExtensions);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ServiceProviderFactoryAdapter");
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Testing.TestConventionContextBuilderExtensions);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ThrowHelper");
                    break;
            }
        }

        private Assembly _RocketSurgeryConventions;
        private Assembly RocketSurgeryConventions => _RocketSurgeryConventions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions, Version=version, Culture=neutral, PublicKeyToken=null"));

        private Assembly _RocketSurgeryConventionsAbstractions;
        private Assembly RocketSurgeryConventionsAbstractions => _RocketSurgeryConventionsAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null"));
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
