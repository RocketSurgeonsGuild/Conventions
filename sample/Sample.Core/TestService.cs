namespace Sample.Core;

[UsedImplicitly]
internal sealed class TestService : IService
{
    public string GetString() => nameof(TestService);
}
