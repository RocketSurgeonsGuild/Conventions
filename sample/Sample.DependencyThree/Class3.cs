using FluentValidation;
using Rocket.Surgery.Conventions;
using Sample.DependencyOne;
using Sample.DependencyThree;

[assembly: Convention(typeof(Class3))]

namespace Sample.DependencyThree;

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