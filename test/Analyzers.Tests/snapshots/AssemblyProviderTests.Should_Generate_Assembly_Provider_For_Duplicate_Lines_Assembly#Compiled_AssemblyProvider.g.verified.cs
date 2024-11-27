//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.g.cs
#nullable enable
#pragma warning disable CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.DependencyInjection.Compiled;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies","")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies","")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies","")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies","")]
[assembly: Rocket.Surgery.Conventions.AssemblyProviderAttribute(typeof(TestProject.Conventions.AssemblyProvider))]
namespace TestProject.Conventions
{
    internal sealed partial class Imports
    {
        public ICompiledTypeProvider CreateAssemblyProvider() => typeof(Imports).GetCompiledTypeProvider();
    }

    [System.CodeDom.Compiler.GeneratedCode("Rocket.Surgery.Conventions.Analyzers", "version"), System.Runtime.CompilerServices.CompilerGenerated, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    file class AssemblyProvider() : ICompiledTypeProvider
    {
        IEnumerable<Assembly> ICompiledTypeProvider.GetAssemblies(Action<IReflectionAssemblySelector> action, int lineNumber, string filePath, string argumentExpression)
        {
            switch (lineNumber)
            {
                // FilePath: Input0.cs Expression: jvHL1AHMsGW7Xy4O6Iiu7w==
                case 7:
                    switch (System.IO.Path.GetFileName(filePath))
                    {
                        // FilePath: Input0.cs Expression: jvHL1AHMsGW7Xy4O6Iiu7w==
                        case "Input0.cs":
                            switch (ICompiledTypeProvider.GetArgumentExpressionHash(argumentExpression))
                            {
                                // FilePath: Input0.cs Expression: jvHL1AHMsGW7Xy4O6Iiu7w==
                                case "jvHL1AHMsGW7Xy4O6Iiu7w==":
                                    yield return typeof(global::TestConvention).Assembly;
                                    break;
                                // FilePath: Folder/Input0.cs Expression: AJD0wI+GHf59jfK+xhPQQg==
                                case "AJD0wI+GHf59jfK+xhPQQg==":
                                    yield return typeof(global::FluentValidation.AbstractValidator<>).Assembly;
                                    yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationDebugViewContext).Assembly;
                                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ActivatorUtilities).Assembly;
                                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContext).Assembly;
                                    yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute).Assembly;
                                    yield return typeof(global::Dep1.Dep1Exports).Assembly;
                                    yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                                    yield return typeof(global::Dep2Exports).Assembly;
                                    yield return typeof(global::System.IServiceProvider).Assembly;
                                    yield return typeof(global::TestConvention).Assembly;
                                    break;
                            }

                            break;
                        // FilePath: Input1.cs Expression: AJD0wI+GHf59jfK+xhPQQg==
                        case "Input1.cs":
                            switch (ICompiledTypeProvider.GetArgumentExpressionHash(argumentExpression))
                            {
                                // FilePath: Input1.cs Expression: AJD0wI+GHf59jfK+xhPQQg==
                                case "AJD0wI+GHf59jfK+xhPQQg==":
                                    yield return typeof(global::FluentValidation.AbstractValidator<>).Assembly;
                                    yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationDebugViewContext).Assembly;
                                    yield return typeof(global::Microsoft.Extensions.DependencyInjection.ActivatorUtilities).Assembly;
                                    yield return typeof(global::Rocket.Surgery.Conventions.ConventionContext).Assembly;
                                    yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute).Assembly;
                                    yield return typeof(global::Dep1.Dep1Exports).Assembly;
                                    yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                                    yield return typeof(global::Dep2Exports).Assembly;
                                    yield return typeof(global::System.IServiceProvider).Assembly;
                                    yield return typeof(global::TestConvention).Assembly;
                                    break;
                                // FilePath: Folder/Input1.cs Expression: jvHL1AHMsGW7Xy4O6Iiu7w==
                                case "jvHL1AHMsGW7Xy4O6Iiu7w==":
                                    yield return typeof(global::TestConvention).Assembly;
                                    break;
                            }

                            break;
                    }

                    break;
            }
        }

        IEnumerable<Type> ICompiledTypeProvider.GetTypes(Func<IReflectionTypeSelector, IEnumerable<Type>> selector, int lineNumber, string filePath, string argumentExpression)
        {
            yield break;
        }
    }
}
#pragma warning restore CA1002, CA1034, CA1822, CS0105, CS1573, CS8602, CS8603, CS8618, CS8669
#nullable restore
