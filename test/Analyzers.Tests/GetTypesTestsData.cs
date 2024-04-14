using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class GetTypesTestsData
{
    public static IEnumerable<object[]> GetTypesData()
    {
        // ReSharper disable RedundantNameQualifier


        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.StartsWith("IService")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.StartsWith("S").EndsWith("Convention")));
        yield return TestMethod(z => z.FromAssemblyDependenciesOf<IConvention>().GetTypes(x => x.StartsWith("T").EndsWith("Convention")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.EndsWith("Convention")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").KindOf(TypeKindFilter.Class)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").KindOf(TypeKindFilter.Interface)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").KindOf(TypeKindFilter.Delegate)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").KindOf(TypeKindFilter.Enum)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Class)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Interface)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Delegate)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Enum)));
        yield return TestMethod(
            z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Delegate, TypeKindFilter.Class))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Delegate, TypeKindFilter.Class, TypeKindFilter.Enum))
        );
        yield return TestMethod(
            z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Delegate).NotKindOf(TypeKindFilter.Interface))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.InfoOf(TypeInfoFilter.Abstract).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.InfoOf(TypeInfoFilter.Visible).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.InfoOf(TypeInfoFilter.ValueType).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
//        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.InfoOf(TypeInfoFilter.Nested).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.InfoOf(TypeInfoFilter.Sealed).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.InfoOf(TypeInfoFilter.GenericType).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
//        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.InfoOf(TypeInfoFilter.GenericTypeDefinition).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.NotInfoOf(TypeInfoFilter.Abstract).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.NotInfoOf(TypeInfoFilter.Visible).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.NotInfoOf(TypeInfoFilter.ValueType).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
//        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotInfoOf(TypeInfoFilter.Nested).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.NotInfoOf(TypeInfoFilter.Sealed).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.NotInfoOf(TypeInfoFilter.GenericType).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations"))
        );
//        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotInfoOf(TypeInfoFilter.GenericTypeDefinition).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.AssignableTo(typeof(IConvention))));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.AssignableTo<IConvention>()));
        yield return TestMethod(
            z => z
                .FromAssemblies()
                .GetTypes(
                     x => x.AssignableToAny(
                         typeof(IServiceConvention),
                         typeof(IServiceAsyncConvention),
                         typeof(IConfigurationConvention),
                         typeof(IConfigurationAsyncConvention)
                     )
                 )
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.NotAssignableTo(typeof(IConvention)).NotAssignableTo(typeof(Attribute)).NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(x => x.NotAssignableTo<IConvention>().NotAssignableTo(typeof(Attribute)).NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .FromAssemblyOf<ConventionContext>()
                .GetTypes(
                     x => x
                         .NotAssignableToAny(
                              typeof(IServiceConvention),
                              typeof(IServiceAsyncConvention),
                              typeof(IConfigurationConvention),
                              typeof(IConfigurationAsyncConvention),
                              typeof(Attribute)
                          )
                         .NotInNamespaces("JetBrains.Annotations")
                 )
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .FromAssemblyOf<ConventionContext>()
                .GetTypes(x => x.WithAttribute(typeof(EditorBrowsableAttribute)))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .FromAssemblyOf<ConventionContext>()
                .GetTypes(x => x.WithAttribute<EditorBrowsableAttribute>())
        );
        yield return TestMethod(
            z => z.FromAssemblyOf<IConvention>().FromAssemblyOf<ConventionContext>().GetTypes(x => x.WithAttribute("JetBrains.Annotations.PublicAPIAttribute"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .FromAssemblyOf<ConventionContext>()
                .GetTypes(x => x.WithAttribute(typeof(System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute).FullName))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .FromAssemblyOf<ConventionContext>()
                .GetTypes(
                     x => x
                         .WithoutAttribute(typeof(EditorBrowsableAttribute))
                         .NotAssignableTo(typeof(Attribute))
                         .NotInNamespaces("JetBrains.Annotations")
                 )
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .FromAssemblyOf<ConventionContext>()
                .GetTypes(
                     x => x
                         .WithoutAttribute<EditorBrowsableAttribute>()
                         .NotAssignableTo<Attribute>()
                         .NotInNamespaces("JetBrains.Annotations")
                 )
        );
//        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().FromAssemblyOf<ConventionContext>().GetTypes(x => x.WithoutAttribute(typeof(JetBrains.Annotations.PublicAPIAttribute).FullName).NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.InNamespaceOf(typeof(ConfigurationExtensions))));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.InNamespaceOf<IConfiguration>()));
        yield return TestMethod(
            z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotInNamespaceOf(typeof(IServiceConvention)).NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotInNamespaceOf<IServiceConvention>().NotInNamespaces("JetBrains.Annotations"))
        );
        yield return TestMethod(
            z => z.FromAssemblies().GetTypes(x => x.InNamespaces("Microsoft.Extensions.Configuration", "Microsoft.Extensions.DependencyInjection"))
        );
        yield return TestMethod(
            z => z
                .FromAssemblyOf<IConvention>()
                .GetTypes(
                     true,
                     x => x.NotInNamespaces("Rocket.Surgery.Conventions.DependencyInjection", "Rocket.Surgery.Conventions.Reflection", "JetBrains.Annotations")
                 )
        );

        static object[] TestMethod(
            Func<ITypeProviderAssemblySelector, IEnumerable<Type>> func,
            [CallerArgumentExpression(nameof(func))]
            string argument = null!
        )
        {
            return [new GetTypesItem(argument[( argument.LastIndexOf("=> x") + 5 )..^1].Replace("\r", "").Replace("\n", "").Trim(), argument, func),];
        }
    }

    public record GetTypesItem(string Name, string Expression, Func<ITypeProviderAssemblySelector, IEnumerable<Type>> Selector)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}