namespace Sample.Core
{
    internal class TestService : IService
    {
        public string GetString() => nameof(TestService);
    }
}