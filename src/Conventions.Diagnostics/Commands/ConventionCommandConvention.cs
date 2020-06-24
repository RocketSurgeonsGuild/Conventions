using Rocket.Surgery.Conventions.CommandLine;

namespace Rocket.Surgery.Conventions.Diagnostics.Commands
{
    internal class ConventionCommandConvention : ICommandLineConvention
    {
        public void Register(ICommandLineConventionContext context)
        {
            var command = context.AddCommand<DiagnosticsCommand>();
            command.Command<ConventionCommand>("conventions", app => { });
        }
    }
}