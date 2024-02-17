using System.Collections.Immutable;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public static class GenerationHelpers
{
    public static CSharpCompilation[] CreateDeps()
    {
        var baseBuilder = GeneratorTestContextBuilder
                         .Create()
                         .AddCommonReferences()
                         .AddCommonGenerators();
        var c1 = Class1(baseBuilder);
        var c2 = Class2(baseBuilder);
        var c3 = Class3(baseBuilder, c1);
        return new[] { c1, c2, c3 };
    }

    public static CSharpCompilation Class1(GeneratorTestContextBuilder builder)
    {
        return builder
              .WithProjectName("SampleDependencyOne")
              .AddSources(
                   @"using Rocket.Surgery.Conventions;
using Sample.DependencyOne;

[assembly: ExportConventions(Namespace = ""Dep1"", ClassName = ""Dep1Exports"")]
[assembly: Convention(typeof(Class1))]

namespace Sample.DependencyOne;

public class Class1 : IConvention
{
}
"
               )
              .Build()
              .Compile();
    }

    public static CSharpCompilation Class2(GeneratorTestContextBuilder builder)
    {
        return builder
              .WithProjectName("SampleDependencyTwo")
              .AddSources(
                   @"using Rocket.Surgery.Conventions;
using Sample.DependencyTwo;

[assembly: ExportConventions(Namespace = null, ClassName = ""Dep2Exports"")]
[assembly: Convention(typeof(Class2))]

namespace Sample.DependencyTwo;

public class Class2 : IConvention
{
}"
               )
              .Build()
              .Compile();
    }

    public static CSharpCompilation Class3(GeneratorTestContextBuilder builder, CSharpCompilation class1)
    {
        return builder
              .WithProjectName("SampleDependencyThree")
              .AddCompilationReferences(class1)
              .AddSources(@"using Rocket.Surgery.Conventions;
using Sample.DependencyOne;
using Sample.DependencyThree;

[assembly: Convention(typeof(Class3))]

namespace Sample.DependencyThree;

public class Class3 : IConvention
{
    public Class1? Class1 { get; set; }
}
"
               )
              .Build()
              .Compile();
    }
}
