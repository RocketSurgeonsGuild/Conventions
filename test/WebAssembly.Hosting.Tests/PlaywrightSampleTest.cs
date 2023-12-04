using FluentAssertions;
using Microsoft.Playwright;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

[UsesVerify]
public class PlaywrightSampleTest : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _playwrightFixture;

    public PlaywrightSampleTest(PlaywrightFixture playwrightFixture)
    {
        _playwrightFixture = playwrightFixture;
    }

    [Fact]
    public async Task LetsGo()
    {
        var browser = _playwrightFixture.Browser;
        var page = await browser.NewPageAsync();
        await page.GotoAsync(_playwrightFixture.Uri);
        await page.WaitForSelectorAsync(".loading-progress", new () { State = WaitForSelectorState.Detached });
        await Verify(page);
    }
}