using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public static class GenerationHelpers
{
    public static async Task<GeneratorTestResults[]> CreateDeps(GeneratorTestContextBuilder rootBuilder)
    {
        var baseBuilder = rootBuilder;
        var c1 = await Class1(baseBuilder);
        var c2 = await Class2(baseBuilder);
        var c3 = await Class3(baseBuilder, c1);
        return new[] { c1, c2, c3, };
    }

    public static async Task<GeneratorTestResults[]> CreateGenericDeps(GeneratorTestContextBuilder rootBuilder)
    {
        var baseBuilder = rootBuilder;
        var c1 = await GenericClass1(baseBuilder);
        var c2 = await GenericClass2(baseBuilder);
        var c3 = await GenericClass3(baseBuilder, c1);
        return new[] { c1, c2, c3, };
    }

    public static Task<GeneratorTestResults> Class1(GeneratorTestContextBuilder builder)
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
              .GenerateAsync();
    }

    public static Task<GeneratorTestResults> Class2(GeneratorTestContextBuilder builder)
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
              .GenerateAsync();
    }

    public static Task<GeneratorTestResults> Class3(GeneratorTestContextBuilder builder, GeneratorTestResults class1)
    {
        return builder
              .WithProjectName("SampleDependencyThree")
              .AddCompilationReferences(class1)
              .AddSources(
                   @"using Rocket.Surgery.Conventions;
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
              .GenerateAsync();
    }

    public static Task<GeneratorTestResults> GenericClass1(GeneratorTestContextBuilder builder)
    {
        return builder
              .WithProjectName("SampleDependencyOne")
              .AddSources(
                   @"using Rocket.Surgery.Conventions;
using Sample.DependencyOne;

[assembly: ExportConventions(Namespace = ""Dep1"", ClassName = ""Dep1Exports"")]
[assembly: Convention<Class1>]

namespace Sample.DependencyOne;

public class Class1 : IConvention
{
}
"
               )
              .Build()
              .GenerateAsync();
    }

    public static Task<GeneratorTestResults> GenericClass2(GeneratorTestContextBuilder builder)
    {
        return builder
              .WithProjectName("SampleDependencyTwo")
              .AddSources(
                   @"using Rocket.Surgery.Conventions;
using Sample.DependencyTwo;

[assembly: ExportConventions(Namespace = null, ClassName = ""Dep2Exports"")]
[assembly: Convention<Class2>]

namespace Sample.DependencyTwo;

public class Class2 : IConvention
{
}"
               )
              .Build()
              .GenerateAsync();
    }

    public static Task<GeneratorTestResults> GenericClass3(GeneratorTestContextBuilder builder, GeneratorTestResults class1)
    {
        return builder
              .WithProjectName("SampleDependencyThree")
              .AddCompilationReferences(class1)
              .AddSources(
                   @"using Rocket.Surgery.Conventions;
using Sample.DependencyOne;
using Sample.DependencyThree;

[assembly: Convention<Class3>]

namespace Sample.DependencyThree;

public class Class3 : IConvention
{
    public Class1? Class1 { get; set; }
}
"
               )
              .Build()
              .GenerateAsync();
    }
}