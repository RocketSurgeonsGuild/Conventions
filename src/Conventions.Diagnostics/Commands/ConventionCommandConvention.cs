using Rocket.Surgery.Conventions.CommandLine;

namespace Rocket.Surgery.Conventions.Diagnostics.Commands;

internal class ConventionCommandConvention : ICommandLineConvention
{
    public void Register(IConventionContext context, ICommandLineContext commandLineContext)
    {
        var command = commandLineContext.AddCommand<DiagnosticsCommand>();
        command.Command<ConventionCommand>("conventions", app => { });
    }
}
