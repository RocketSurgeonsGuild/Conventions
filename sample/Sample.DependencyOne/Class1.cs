using FluentValidation;
using Rocket.Surgery.Conventions;
using Sample.DependencyOne;

[assembly: ExportConventions(Namespace = "Dep1", ClassName = "Dep1Exports")]
[assembly: Convention<Class1>]

namespace Sample.DependencyOne;

public class Class1 : IConvention;

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