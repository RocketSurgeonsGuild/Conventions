namespace Clavus.Analyzers.Tests;

/// <summary>
///     Task 3.4: adversarial-input Verify snapshot tests for the type-inference precedence chain
///     defined in design.md Decision 3: `TimeSpan` -> `DateOnly` -> `TimeOnly` -> `DateTimeOffset`
///     -> primitive fallback, each anchored to a strict format regex before parsing (to avoid the
///     `"14:30"` `TimeOnly`/`TimeSpan` ambiguity called out as a risk in design.md).
///
///     Each case below is a single-value config file so the generated class's single inferred
///     property type is the entire point of the snapshot - the Verify snapshot is the source of
///     truth for "what type did the generator choose for this raw string."
/// </summary>
public class TypeInferenceAdversarialTests() : ConfigGeneratorTest()
{
    [Test]
    public async Task Should_Infer_TimeSpan_Not_TimeOnly_For_24_Hour_Boundary()
    {
        // "24:00" is not a valid TimeOnly (hour must be 0-23) but is a valid TimeSpan duration
        // (24 hours). Regression case for the design.md risk: ambiguous colon-delimited strings.
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "24:00" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Infer_TimeSpan_For_Day_Prefixed_Duration()
    {
        // "1.00:00:00" - the `d.hh:mm:ss` shape is TimeSpan-only; DateOnly/TimeOnly never match it.
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "1.00:00:00" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Infer_TimeSpan_For_Sub_Second_Fractional_Duration()
    {
        // "00:00:00.5" - fractional-seconds duration shape, still TimeSpan per Decision 3 step 1.
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "00:00:00.5" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Infer_DateOnly_For_Iso_Date()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "2024-01-01" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Infer_TimeOnly_For_Hour_Minute_No_Date_Component()
    {
        // "14:30" - the canonical ambiguous case: a valid TimeOnly, but does NOT match the
        // TimeSpan step's stricter anchored shape (no `d.` prefix, no seconds component
        // required to be TimeSpan-only), so precedence must land on TimeOnly here.
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "14:30" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Infer_TimeOnly_With_Fractional_Seconds()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "14:30:00.123" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Infer_DateTimeOffset_For_Iso_DateTime_With_Zulu_Offset()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "2024-01-01T14:30:00Z" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Infer_DateTimeOffset_For_Iso_DateTime_With_Numeric_Offset()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "2024-01-01T14:30:00.500-05:00" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Fall_Back_To_String_For_Invalid_Calendar_Date()
    {
        // "2024-02-30" matches the ISO-date *shape* but February has no 30th - the strict
        // anchored-regex-then-parse approach (Decision 3) must reject this at the parse step
        // and fall through to the string fallback rather than throwing or emitting DateOnly.
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText("appsettings.json", /*lang=json,strict*/ """{ "Value": "2024-02-30" }""")
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Fall_Back_To_Primitive_Chain_For_Bool_Int_Double_And_Plain_String()
    {
        var result = await WithSharedDeps()
                          .AddGlobalOption("build_property.EnableClavusConfiguration", "true")
                          .AddAdditionalText(
                               "appsettings.json",
                               /*lang=json,strict*/
                               """
                               {
                                 "IsEnabled": "true",
                                 "Count": "42",
                                 "Ratio": "3.14",
                                 "Name": "not-a-recognized-shape"
                               }
                               """
                           )
                          .AddOption("appsettings.json", "build_metadata.AdditionalFiles.ClavusConfigFormat", "Json")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }
}
