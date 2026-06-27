using FluentValidation;
using Sample.DependencyOne;

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
