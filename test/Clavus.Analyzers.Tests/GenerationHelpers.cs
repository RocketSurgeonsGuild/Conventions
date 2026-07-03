using System.Linq.Expressions;
using FluentValidation;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Clavus.Analyzers.Tests;

public static class GenerationHelpers
{
    public static async Task<GeneratorTestResults[]> CreateDeps(GeneratorTestContextBuilder rootBuilder, CancellationToken cancellationToken)
    {
        var baseBuilder = rootBuilder.AddReferences(typeof(IValidator).Assembly, typeof(Expression<>).Assembly);
        var c1 = await Class1(baseBuilder, cancellationToken);
        var c2 = await Class2(baseBuilder, cancellationToken);
        var c3 = await Class3(baseBuilder, c1, cancellationToken);
        return [c1, c2, c3,];
    }

    public static async Task<GeneratorTestResults[]> CreateGenericDeps(GeneratorTestContextBuilder rootBuilder, CancellationToken cancellationToken)
    {
        var baseBuilder = rootBuilder;
        var c1 = await GenericClass1(baseBuilder, cancellationToken);
        var c2 = await GenericClass2(baseBuilder, cancellationToken);
        var c3 = await GenericClass3(baseBuilder, c1, cancellationToken);
        return [c1, c2, c3,];
    }

    public static Task<GeneratorTestResults> Class1(GeneratorTestContextBuilder builder, CancellationToken cancellationToken)
    {
        return builder
              .WithProjectName("SampleDependencyOne")
              .AddSources(
                   @"using Clavus;
using Sample.DependencyOne;
using FluentValidation;

[assembly: ExportClavusParts(Namespace = ""Dep1"", ClassName = ""Dep1Exports"")]

namespace Sample.DependencyOne;

[ExportClavusPart]
public class Class1 : IClavusPart
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
              .GenerateAsync(cancellationToken);
    }

    public static Task<GeneratorTestResults> Class2(GeneratorTestContextBuilder builder, CancellationToken cancellationToken)
    {
        return builder
              .WithProjectName("SampleDependencyTwo")
              .AddSources(
                   @"using Clavus;
using FluentValidation;

[assembly: ExportClavusParts(Namespace = null, ClassName = ""Dep2Exports"")]
namespace Sample.DependencyTwo;

public static class Nested
{
    [ExportClavusPart]
    public class Class2 : IClavusPart;
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
              .GenerateAsync(cancellationToken);
    }

    public static Task<GeneratorTestResults> Class3(GeneratorTestContextBuilder builder, GeneratorTestResults class1, CancellationToken cancellationToken)
    {
        return builder
              .WithProjectName("SampleDependencyThree")
              .AddCompilationReferences(class1)
              .AddSources(
                   @"using Clavus;
using Sample.DependencyOne;
using Sample.DependencyThree;
using FluentValidation;

namespace Sample.DependencyThree;

[ExportClavusPart]
public class Class3 : IClavusPart
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
              .GenerateAsync(cancellationToken);
    }

    public static Task<GeneratorTestResults> GenericClass1(GeneratorTestContextBuilder builder, CancellationToken cancellationToken)
    {
        return builder
              .WithProjectName("SampleDependencyOne")
              .AddSources(
                   @"using Clavus;
using Sample.DependencyOne;

[assembly: ExportClavusParts(Namespace = ""Dep1"", ClassName = ""Dep1Exports"")]

namespace Sample.DependencyOne;

[ExportClavusPart]
public class Class1 : IClavusPart
{
}
"
               )
              .Build()
              .GenerateAsync(cancellationToken);
    }

    public static Task<GeneratorTestResults> GenericClass2(GeneratorTestContextBuilder builder, CancellationToken cancellationToken)
    {
        return builder
              .WithProjectName("SampleDependencyTwo")
              .AddSources(
                   @"using Clavus;
using Sample.DependencyTwo;

[assembly: ExportClavusParts(Namespace = null, ClassName = ""Dep2Exports"")]

namespace Sample.DependencyTwo;

[ExportClavusPart]
public class Class2 : IClavusPart
{
}"
               )
              .Build()
              .GenerateAsync(cancellationToken);
    }

    public static Task<GeneratorTestResults> GenericClass3(GeneratorTestContextBuilder builder, GeneratorTestResults class1, CancellationToken cancellationToken)
    {
        return builder
              .WithProjectName("SampleDependencyThree")
              .AddCompilationReferences(class1)
              .AddSources(
                   @"using Clavus;
using Sample.DependencyOne;
using Sample.DependencyThree;

namespace Sample.DependencyThree;

[ExportClavusPart]
public class Class3 : IClavusPart
{
    public Class1? Class1 { get; set; }
}
"
               )
              .Build()
              .GenerateAsync(cancellationToken);
    }
}
