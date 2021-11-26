using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;

namespace Sample;

[ImportConventions]
public static partial class Program
{
    public static Task<int> Main(string[] args)
    {
        return App.Create<DefaultCommand>().RunAsync(args);
    }
}
