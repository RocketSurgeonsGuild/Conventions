//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using System.Runtime.Loader;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
[assembly: Rocket.Surgery.Conventions.AssemblyProviderAttribute(typeof(TestProject.Conventions.AssemblyProvider))]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public IAssemblyProvider CreateAssemblyProvider() => new AssemblyProvider(builder.Properties.GetRequiredService<AssemblyLoadContext>());
    }

    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    file class AssemblyProvider(AssemblyLoadContext context) : IAssemblyProvider
    {
        IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
        {
            yield break;
        }

        IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
        {
            switch (lineNumber)
            {
                // FilePath: Input0.cs Expression: 69/MqJ7BQbbNED1xa1MUMw==
                case 18:
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
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ActivatorUtilities);
                    yield return MicrosoftExtensionsDependencyInjectionAbstractions.GetType("Microsoft.Extensions.DependencyInjection.ActivatorUtilities+ActivatorUtilitiesUpdateHandler");
                    yield return MicrosoftExtensionsDependencyInjectionAbstractions.GetType("Microsoft.Extensions.DependencyInjection.ActivatorUtilities+ConstructorInfoEx");
                    yield return MicrosoftExtensionsDependencyInjectionAbstractions.GetType("Microsoft.Extensions.DependencyInjection.ActivatorUtilities+ConstructorMatcher");
                    yield return MicrosoftExtensionsDependencyInjectionAbstractions.GetType("Microsoft.Extensions.DependencyInjection.ActivatorUtilities+FactoryParameterContext");
                    yield return MicrosoftExtensionsDependencyInjectionAbstractions.GetType("Microsoft.Extensions.DependencyInjection.ActivatorUtilities+StackAllocatedObjects");
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructorAttribute);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.AsyncServiceScope);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.FromKeyedServicesAttribute);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.IKeyedServiceProvider);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.IServiceCollection);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.IServiceProviderFactory<>);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.IServiceProviderIsKeyedService);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.IServiceProviderIsService);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.IServiceScope);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.IServiceScopeFactory);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ISupportRequiredService);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.KeyedService);
                    yield return MicrosoftExtensionsDependencyInjectionAbstractions.GetType("Microsoft.Extensions.DependencyInjection.KeyedService+AnyKeyObj");
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ObjectFactory);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ObjectFactory<>);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ServiceCollection);
                    yield return MicrosoftExtensionsDependencyInjectionAbstractions.GetType("Microsoft.Extensions.DependencyInjection.ServiceCollection+ServiceCollectionDebugView");
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ServiceKeyAttribute);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ServiceLifetime);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ServiceProviderKeyedServiceExtensions);
                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions);
                    break;
            }
        }

        private Assembly _MicrosoftExtensionsDependencyInjectionAbstractions;
        private Assembly MicrosoftExtensionsDependencyInjectionAbstractions => _MicrosoftExtensionsDependencyInjectionAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Microsoft.Extensions.DependencyInjection.Abstractions, Version=version, Culture=neutral, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f33a29044fa9d740c9b3213a93e57c84b472c84e0b8a0e1ae48e67a9f8f6de9d5f7f3d52ac23e48ac51801f1dc950abe901da34d2a9e3baadb141a17c77ef3c565dd5ee5054b91cf63bb3c6ab83f72ab3aafe93d0fc3c2348b764fafb0b1c0733de51459aeab46580384bf9d74c4e28164b7cde247f891ba07891c9d872ad2bb"));
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
