using DryIoc;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

internal static class AppExtensions
{
    public static IContainer GetLifetimeScope(this ICommandApp builder)
    {
        IContainer scope = null!;
        builder.Configure(
            z =>
            {
                z.Settings.Registrar.RegisterInstance<Action<IContainer>>(
                    services => { scope = services; }
                );
                z.AddCommand<DelegateCommand>("test");
            }
        );
        builder.Run(new[] { "test" });
        return scope!;
    }

    private class DelegateCommand : Command
    {
        private readonly IContainer _scope;
        private readonly Action<IContainer> _action;

        public DelegateCommand(IContainer scope, Action<IContainer> action)
        {
            _scope = scope;
            _action = action;
        }

        public override int Execute(CommandContext context)
        {
            _action(_scope);
            return 0;
        }
    }
}
