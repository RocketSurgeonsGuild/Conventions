using System.Reflection;
using System.Runtime.CompilerServices;
using DiffEngine;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyGeneratorTextContext.Initialize(Customizers.Default);

        DiffRunner.Disabled = true;
        DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                static string GetTypeName(Type type)
                {
                    return type.IsNested ? $"{type.ReflectedType!.Name}.{type.Name}" : type.Name;
                }

                var typeName = GetTypeName(type);

                var path = Path.Combine(Path.GetDirectoryName(sourceFile)!, "snapshots");
                return new(path, typeName, method.Name);
            }
        );

        VerifierSettings.ScrubLines(z => z.Contains("Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.", StringComparison.OrdinalIgnoreCase));
        VerifierSettings.AddScrubber(
            (builder, counter) =>
            {
                if (typeof(ConventionAttributesGenerator).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>() is { Version: { Length: > 0, } version, })
                    builder.Replace(version, "version");
            }
        );
    }
}
