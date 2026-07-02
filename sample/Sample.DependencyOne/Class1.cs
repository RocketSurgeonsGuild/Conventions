using FluentValidation;

[assembly: ExportClavusParts(Namespace = "Dep1", ClassName = "Dep1Exports")]

namespace Sample.DependencyOne;

[ExportClavusPart]
public class Class1 : IClavusPart;

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
