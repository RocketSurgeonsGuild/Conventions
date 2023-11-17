namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

[CollectionDefinition(nameof(PlaywrightFixture))]
public class SharedPlaywrightCollection : ICollectionFixture<PlaywrightFixture> {}
