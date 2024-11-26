//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using System.Runtime.Loader;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes","")]
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
                // FilePath: Input0.cs Expression: JYYBZ/BE9++owK7IEqAUsg==
                case 18:
                    yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionCategory);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionCategoryAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilder);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextExtensions);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionDependency");
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionHostBuilderExtensions);
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionMetadata);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.ConventionProvider");
                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionsConfigurationAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependentOfConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependentOfConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependsOnConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.DependsOnConventionAttribute<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.ExportConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ExportConventionsAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.ExportedConventionsAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedAsyncConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.HostCreatedConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.IHostCreatedAsyncConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.Hosting.IHostCreatedConvention<>);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionContext);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionDependency);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionFactory);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionMetadata);
                    yield return typeof(global::Rocket.Surgery.Conventions.IConventionProvider);
                    yield return RocketSurgeryConventionsAbstractions.GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                    yield return typeof(global::Rocket.Surgery.Conventions.ImportConventionsAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.LiveConventionAttribute);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                    yield return typeof(global::Rocket.Surgery.Conventions.UnitTestConventionAttribute);
                    break;
            }
        }

        private Assembly _RocketSurgeryConventionsAbstractions;
        private Assembly RocketSurgeryConventionsAbstractions => _RocketSurgeryConventionsAbstractions ??= context.LoadFromAssemblyName(new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null"));
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
