namespace Sample.Core;

[UsedImplicitly]
internal sealed class AService : IService
{
    public string GetString()
    {
        return nameof(AService);
    }
}
