//using McMaster.Extensions.CommandLineUtils;
//
//namespace Clavus.Hosting.Tests;
//
//[Command]
//[UsedImplicitly]
//internal class MyCommand
//{
//    [UsedImplicitly] private readonly IServiceProvider _serviceProvider;
//
//    public MyCommand(IServiceProvider serviceProvider)
//    {
//        _serviceProvider =
//            serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
//    }
//
//    public Task<int> OnExecuteAsync()
//    {
//        return Task.FromResult(1234);
//    }
//}
