using Microsoft.Playwright;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

public class PlaywrightSampleTest(PlaywrightFixture playwrightFixture) : IClassFixture<PlaywrightFixture>
{
    [Fact]
    public async Task LetsGo()
    {
        var browser = playwrightFixture.Browser;
        var page = await browser.NewPageAsync();
        await page.GotoAsync(playwrightFixture.Uri);
        await page.WaitForSelectorAsync(".loading-progress", new() { State = WaitForSelectorState.Detached, });
        await Verify(page);
    }
}
