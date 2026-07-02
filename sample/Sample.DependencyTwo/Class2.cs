using FluentValidation;

[assembly: ExportClavusParts(Namespace = null, ClassName = "Dep2Exports")]

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
