using Autofac;
using Spectre.Console.Cli;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

internal static class AppExtensions
{
    public static ILifetimeScope GetLifetimeScope(this ICommandApp builder)
    {
        ILifetimeScope scope = null!;
        builder.Configure(
            z =>
            {
                z.Settings.Registrar.RegisterInstance<Action<ILifetimeScope>>(
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
        private readonly ILifetimeScope _scope;
        private readonly Action<ILifetimeScope> _action;

        public DelegateCommand(ILifetimeScope scope, Action<ILifetimeScope> action)
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
