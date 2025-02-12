using System.Linq.Expressions;
using FluentValidation;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public static class GenerationHelpers
{
    public static async Task<GeneratorTestResults[]> CreateDeps(GeneratorTestContextBuilder rootBuilder)
    {
        var baseBuilder = rootBuilder.AddReferences(typeof(IValidator).Assembly, typeof(Expression<>).Assembly);
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
using FluentValidation;

[assembly: ExportConventions(Namespace = ""Dep1"", ClassName = ""Dep1Exports"")]

namespace Sample.DependencyOne;

[ExportConvention]
public class Class1 : IConvention
{
}

public static class Example1
{
    public record Request(string A, double B);

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.A).NotEmpty();
            RuleFor(x => x.B).GreaterThan(0);
        }
    }
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
using FluentValidation;

[assembly: ExportConventions(Namespace = null, ClassName = ""Dep2Exports"")]
namespace Sample.DependencyTwo;

public static class Nested
{
    [ExportConvention]
    public class Class2 : IConvention;
}

public static class Example2
{
    public record Request(string A, double B);

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.A).NotEmpty();
            RuleFor(x => x.B).GreaterThan(0);
        }
    }
}

"
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
using FluentValidation;

namespace Sample.DependencyThree;

[ExportConvention]
public class Class3 : IConvention
{
    public Class1? Class1 { get; set; }
}

public static class Example3
{
    public record Request(string A, double B);

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.A).NotEmpty();
            RuleFor(x => x.B).GreaterThan(0);
        }
    }
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

namespace Sample.DependencyOne;

[ExportConvention]
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

namespace Sample.DependencyTwo;

[ExportConvention]
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

namespace Sample.DependencyThree;

[ExportConvention]
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
