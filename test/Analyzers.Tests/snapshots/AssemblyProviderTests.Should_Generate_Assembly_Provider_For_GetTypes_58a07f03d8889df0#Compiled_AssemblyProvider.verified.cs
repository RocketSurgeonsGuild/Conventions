﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetTypes", "eyJsIjp7ImwiOjE0LCJmIjoiSW5wdXQxLmNzIiwibSI6IlJlZ2lzdGVyIn0sImEiOnsiYSI6ZmFsc2UsImkiOmZhbHNlLCJtIjpbIlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zLkFic3RyYWN0aW9ucywgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwiXSwibmEiOltdLCJkIjpbXX0sInQiOnsiZiI6MSwibnNmIjpbXSwibmYiOlt7ImYiOjIsIm4iOlsiQ29udmVudGlvbiJdfV0sImsiOlt7ImYiOmZhbHNlLCJ0IjpbMl19XSwidyI6W10sInMiOltdLCJhdCI6W10sInRhIjpbXSwiYSI6ZmFsc2UsImkiOmZhbHNlLCJtIjpbIlJvY2tldC5TdXJnZXJ5LkNvbnZlbnRpb25zLkFic3RyYWN0aW9ucywgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwiXSwibmEiOltdLCJkIjpbXX19")]
namespace TestProject.Conventions
{
    internal static partial class Imports
    {
        private class AssemblyProvider(AssemblyLoadContext context) : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 202:
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions).Assembly;
                        yield return typeof(global::Dep1.Dep1Exports).Assembly;
                        yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                        yield return typeof(global::Dep2Exports).Assembly;
                        yield return typeof(global::TestConvention).Assembly;
                        break;
                }
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 14:
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationConvention);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionDependency");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionProviderFactory);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionDependency);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionWithDependencies);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
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

            private static AssemblyName _RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull;
            private static AssemblyName RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull => _RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull ??= new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null");
        }
    }
}