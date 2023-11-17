using System.Runtime.CompilerServices;
using DiffEngine;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyPlaywright.Initialize(installPlaywright: true);
        VerifyImageMagick.Initialize();
        VerifyImageMagick.RegisterComparers(.05);
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
    }
}
