using System.Runtime.CompilerServices;
using AngleSharp.Dom;
using DiffEngine;
using VerifyTests.AngleSharp;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifyPlaywright.Initialize(true);
        VerifyAngleSharpDiffing.Initialize();
        HtmlPrettyPrint.All(
            list =>
            {
                list.ScrubAttributes(attr => attr.Name.StartsWith("b-") && attr.Name.Length == 12);
                foreach (var comment in list.DescendantsAndSelf<IComment>().Where(z => z.NodeValue == "!").ToArray())
                {
                    comment.Remove();
                }
            }
        );
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
