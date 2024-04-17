using FluentValidation;
using Rocket.Surgery.Conventions;

[assembly: ExportConventions(Namespace = null, ClassName = "Dep2Exports")]

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
