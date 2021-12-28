namespace Sample.Core;

internal class AService : IService
{
    public string GetString()
    {
        return nameof(AService);
    }
}
