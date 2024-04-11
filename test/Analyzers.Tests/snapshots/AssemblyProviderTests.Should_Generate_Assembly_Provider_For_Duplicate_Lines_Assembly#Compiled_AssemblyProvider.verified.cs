//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies", "eyJsIjp7ImwiOjYsImYiOiJJbnB1dDEuY3MiLCJtIjoiUmVnaXN0ZXIifSwiYSI6eyJhIjpmYWxzZSwiaSI6ZmFsc2UsIm0iOlsiVGVzdFByb2plY3QsIFZlcnNpb249MC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1udWxsIl0sIm5hIjpbXSwiZCI6W119fQ==")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.GetAssemblies", "eyJsIjp7ImwiOjYsImYiOiJJbnB1dDIuY3MiLCJtIjoiUmVnaXN0ZXIifSwiYSI6eyJhIjp0cnVlLCJpIjpmYWxzZSwibSI6W10sIm5hIjpbXSwiZCI6W119fQ==")]
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
                    case 6:
                        switch (filePath)
                        {
                            case "Input1.cs":
                                yield return typeof(global::TestConvention).Assembly;
                                break;
                            case "Input2.cs":
                                yield return typeof(global::Microsoft.Extensions.Configuration.ConfigurationDebugViewContext).Assembly;
                                yield return typeof(global::Microsoft.Extensions.DependencyInjection.ActivatorUtilities).Assembly;
                                yield return context.LoadFromAssemblyName(mscorlibVersion4000CultureneutralPublicKey00000000000000000400000000000000);
                                yield return context.LoadFromAssemblyName(netstandardVersion2100CultureneutralPublicKey00240000048000009400000006020000002400005253413100040000010001004b86c4cb78549b34bab61a3b1800e23bfeb5b3ec390074041536a7e3cbd97f5f04cf0f857155a8928eaa29ebfd11cfbbad3ba70efea7bda3226c6a8d370a4cd303f714486b6ebc225985a638471e6ef571cc92a4613c00b8fa65d61ccee0cbe5f36330c9a01f4183559f1bef24cc2917c6d913e3a541333a1d05d9bed22b38cb);
                                yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions).Assembly;
                                yield return typeof(global::Rocket.Surgery.Conventions.AbstractConventionContextBuilderExtensions).Assembly;
                                yield return typeof(global::Dep1.Dep1Exports).Assembly;
                                yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                                yield return typeof(global::Dep2Exports).Assembly;
                                yield return context.LoadFromAssemblyName(SystemVersion4000CultureneutralPublicKey00000000000000000400000000000000);
                                yield return typeof(global::System.IServiceProvider).Assembly;
                                yield return context.LoadFromAssemblyName(SystemCoreVersion4000CultureneutralPublicKey00000000000000000400000000000000);
                                yield return typeof(global::Internal.Console).Assembly;
                                yield return context.LoadFromAssemblyName(SystemRuntimeVersion8000CultureneutralPublicKey002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293);
                                yield return typeof(global::TestConvention).Assembly;
                                break;
                        }

                        break;
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
                yield break;
            }

            private static AssemblyName _mscorlibVersion4000CultureneutralPublicKey00000000000000000400000000000000;
            private static AssemblyName mscorlibVersion4000CultureneutralPublicKey00000000000000000400000000000000 => _mscorlibVersion4000CultureneutralPublicKey00000000000000000400000000000000 ??= new AssemblyName("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKey=00000000000000000400000000000000");

            private static AssemblyName _netstandardVersion2100CultureneutralPublicKey00240000048000009400000006020000002400005253413100040000010001004b86c4cb78549b34bab61a3b1800e23bfeb5b3ec390074041536a7e3cbd97f5f04cf0f857155a8928eaa29ebfd11cfbbad3ba70efea7bda3226c6a8d370a4cd303f714486b6ebc225985a638471e6ef571cc92a4613c00b8fa65d61ccee0cbe5f36330c9a01f4183559f1bef24cc2917c6d913e3a541333a1d05d9bed22b38cb;
            private static AssemblyName netstandardVersion2100CultureneutralPublicKey00240000048000009400000006020000002400005253413100040000010001004b86c4cb78549b34bab61a3b1800e23bfeb5b3ec390074041536a7e3cbd97f5f04cf0f857155a8928eaa29ebfd11cfbbad3ba70efea7bda3226c6a8d370a4cd303f714486b6ebc225985a638471e6ef571cc92a4613c00b8fa65d61ccee0cbe5f36330c9a01f4183559f1bef24cc2917c6d913e3a541333a1d05d9bed22b38cb => _netstandardVersion2100CultureneutralPublicKey00240000048000009400000006020000002400005253413100040000010001004b86c4cb78549b34bab61a3b1800e23bfeb5b3ec390074041536a7e3cbd97f5f04cf0f857155a8928eaa29ebfd11cfbbad3ba70efea7bda3226c6a8d370a4cd303f714486b6ebc225985a638471e6ef571cc92a4613c00b8fa65d61ccee0cbe5f36330c9a01f4183559f1bef24cc2917c6d913e3a541333a1d05d9bed22b38cb ??= new AssemblyName("netstandard, Version=2.1.0.0, Culture=neutral, PublicKey=00240000048000009400000006020000002400005253413100040000010001004b86c4cb78549b34bab61a3b1800e23bfeb5b3ec390074041536a7e3cbd97f5f04cf0f857155a8928eaa29ebfd11cfbbad3ba70efea7bda3226c6a8d370a4cd303f714486b6ebc225985a638471e6ef571cc92a4613c00b8fa65d61ccee0cbe5f36330c9a01f4183559f1bef24cc2917c6d913e3a541333a1d05d9bed22b38cb");

            private static AssemblyName _SystemVersion4000CultureneutralPublicKey00000000000000000400000000000000;
            private static AssemblyName SystemVersion4000CultureneutralPublicKey00000000000000000400000000000000 => _SystemVersion4000CultureneutralPublicKey00000000000000000400000000000000 ??= new AssemblyName("System, Version=4.0.0.0, Culture=neutral, PublicKey=00000000000000000400000000000000");

            private static AssemblyName _SystemCoreVersion4000CultureneutralPublicKey00000000000000000400000000000000;
            private static AssemblyName SystemCoreVersion4000CultureneutralPublicKey00000000000000000400000000000000 => _SystemCoreVersion4000CultureneutralPublicKey00000000000000000400000000000000 ??= new AssemblyName("System.Core, Version=4.0.0.0, Culture=neutral, PublicKey=00000000000000000400000000000000");

            private static AssemblyName _SystemRuntimeVersion8000CultureneutralPublicKey002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293;
            private static AssemblyName SystemRuntimeVersion8000CultureneutralPublicKey002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293 => _SystemRuntimeVersion8000CultureneutralPublicKey002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293 ??= new AssemblyName("System.Runtime, Version=8.0.0.0, Culture=neutral, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293");
        }
    }
}