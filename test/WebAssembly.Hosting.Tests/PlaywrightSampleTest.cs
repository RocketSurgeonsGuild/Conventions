namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

public class PlaywrightSampleTest()
{
    [Test]
    public async Task Dummy_Test() => await Assert.That(true).IsTrue();
    // [Fact(Skip = "sometimes fails")]
    // public async Task LetsGo()
    // {
    //     var browser = playwrightFixture.Browser;
    //     var page = await browser.NewPageAsync();
    //     await page.GotoAsync(playwrightFixture.Uri);
    //     await page.WaitForSelectorAsync(".loading-progress", new() { State = WaitForSelectorState.Detached, });
    //     await Verify(page);
    // }
}
