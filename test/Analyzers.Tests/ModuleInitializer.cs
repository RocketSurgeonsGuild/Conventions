using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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

        VerifierSettings.SortJsonObjects();
        VerifierSettings.ScrubInlineGuids();
        VerifierSettings.ScrubLinesWithReplace(
            s =>
            {
                if (s.Contains(
                        "Rocket.Surgery.ConventionConfigurationData.AssemblyProvider.",
                        StringComparison.OrdinalIgnoreCase
                    ))
                {
                    return s.Substring(0, s.IndexOf('"', s.IndexOf('"') + 1) + 2) + "\"\")]";
                }

                return s;
            }
        );
        VerifierSettings.AddScrubber(
            (builder, counter) =>
            {
                if (typeof(ConventionAttributesGenerator).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>() is { Version: { Length: > 0, } version, })
                    builder.Replace(version, "version");
                if (typeof(ConventionAttributesGenerator).Assembly.GetCustomAttribute<AssemblyVersionAttribute>() is { Version: { Length: > 0, } version2, })
                    builder.Replace(version2, "version");
                // regex to replace the version number in this string Version=12.0.0.0,
                var regex = new Regex("Version=(.*?),", RegexOptions.Compiled);
                var result = regex.Replace(builder.ToString(), "Version=version,");
                builder.Clear();
                builder.Append(result);
            }
        );
    }
}
