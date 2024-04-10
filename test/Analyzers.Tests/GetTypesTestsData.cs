﻿using System.Runtime.CompilerServices;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class GetTypesTestsData
{

    public static IEnumerable<object[]> GetTypesData()
    {
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.StartsWith("IService")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.StartsWith("S").EndsWith("Convention")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.EndsWith("Convention")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").KindOf(TypeKindFilter.Class)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").KindOf(TypeKindFilter.Interface)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").KindOf(TypeKindFilter.Delegate)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Class)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Interface)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Delegate)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Delegate, TypeKindFilter.Class)));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.Contains("Convention").NotKindOf(TypeKindFilter.Delegate).NotKindOf(TypeKindFilter.Interface)));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.AssignableTo(typeof(IConvention))));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.AssignableTo<IConvention>()));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.AssignableToAny(typeof(IServiceConvention), typeof(IServiceAsyncConvention), typeof(IConfigurationConvention), typeof(IConfigurationAsyncConvention))));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotAssignableTo(typeof(IConvention)).NotAssignableTo(typeof(Attribute)).NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotAssignableTo<IConvention>().NotAssignableTo(typeof(Attribute)).NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotAssignableToAny(typeof(IServiceConvention), typeof(IServiceAsyncConvention), typeof(IConfigurationConvention), typeof(IConfigurationAsyncConvention), typeof(Attribute)).NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.WithAttribute(typeof(BeforeConventionAttribute))));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.WithAttribute<AfterConventionAttribute>()));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.WithoutAttribute(typeof(AfterConventionAttribute)).NotAssignableTo(typeof(Attribute)).NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.WithoutAttribute<BeforeConventionAttribute>().NotAssignableTo<Attribute>().NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.InNamespaceOf(typeof(global::Microsoft.Extensions.Configuration.ConfigurationExtensions))));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.InNamespaceOf<global::Microsoft.Extensions.Configuration.IConfiguration>()));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotInNamespaceOf(typeof(IServiceConvention)).NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotInNamespaceOf<IServiceConvention>().NotInNamespaces("JetBrains.Annotations")));
        yield return TestMethod(z => z.FromAssemblies().GetTypes(x => x.InNamespaces("Microsoft.Extensions.Configuration", "Microsoft.Extensions.DependencyInjection")));
        yield return TestMethod(z => z.FromAssemblyOf<IConvention>().GetTypes(x => x.NotInNamespaces("Rocket.Surgery.Conventions.DependencyInjection", "Rocket.Surgery.Conventions.Reflection", "JetBrains.Annotations")));
        static object[] TestMethod(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> func, [CallerArgumentExpression(nameof(func))] string argument = null!) => [new GetTypesItem(argument[(argument.LastIndexOf("=> x")+5)..^1], argument, func)];
    }

    public record GetTypesItem(string Name, string Expression, Func<ITypeProviderAssemblySelector, IEnumerable<Type>> Selector)
    {
        public override string ToString() => Name;
    };
}
