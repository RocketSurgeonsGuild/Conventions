using System.Reflection;
using System.Runtime.CompilerServices;
using DiffEngine;

namespace Rocket.Surgery.Conventions.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
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
        VerifierSettings.AddExtraSettings(settings =>
                                          {
                                              settings.Converters.Add(new AssemblyConverter());
                                              settings.Converters.Add(new TypeConverter());
                                          });
    }

    class AssemblyConverter : WriteOnlyJsonConverter<Assembly>
    {
        public override void Write(VerifyJsonWriter writer, Assembly value)
        {
            writer.WriteValue(value.GetName().Name);
        }
    }
    class TypeConverter : WriteOnlyJsonConverter<Type>
    {
        public override void Write(VerifyJsonWriter writer, Type value)
        {
            writer.WriteValue(value.FullName);
        }
    }
}
