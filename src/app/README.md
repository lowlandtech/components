# LowlandTech.Components.App

This project hosts the component showcase app. The `Showcode` area contains the
interactive component stories and the source-backed code examples shown inside
each `StoryCard`.

## StoryCard Conversion Guide

We are converting StoryCards from inline string snippets to real Razor/C# example
files. The goal is that every visible code tab is copied from a compilable file,
so Visual Studio understands the example, users see full context, and the preview
cannot drift away from the shown code.

Use `Showcode/Stories/Navigation/TreeViewStory.razor` as the reference pattern.

## Target Structure

For a story at:

```text
Showcode/Stories/{Group}/{Component}Story.razor
```

place real examples at:

```text
Showcode/Examples/{Group}/{Component}/{Component}{StateName}Example.razor
Showcode/Examples/{Group}/{Component}/{Component}SampleData.cs
Showcode/Examples/{Group}/{Component}/{Component}ExampleModel.cs
```

Only add `.cs` files when the example needs shared models, sample data, or helper
types. Keep small one-off state in the `.razor` example.

## Conversion Steps

1. Pick one story file and convert only that story.
2. Create one real `.razor` example component for each meaningful StoryCard state.
3. Move shared sample data or model types into real `.cs` files beside the examples.
4. Update `src/components/app/_Imports.razor` if the story needs a new examples namespace.
5. Replace the story preview fragment with the real example component.
6. Replace inline code strings with `StorySourceRegistry.CodeTab(...)`.
7. Run the source generator script.
8. Build the app project.
9. Add or update an E2E guard when the story introduces a new source-backed pattern.

## Example Story Pattern

```razor
@attribute [DisplayName("Navigation/Example")]
@inherits StoryComponent

<StoryCard Title="Example"
           Description="Example demonstrates the component in common states."
           States="@States" />

@code {
    private const string SampleDataSource = "Showcode/Examples/Navigation/Example/ExampleSampleData.cs";

    private IReadOnlyList<StoryCard.State> States =>
    [
        new(
            "default",
            "Default",
            @<ExampleDefaultExample />,
            [
                StorySourceRegistry.CodeTab(
                    "default-razor",
                    "ExampleDefaultExample.razor",
                    "Showcode/Examples/Navigation/Example/ExampleDefaultExample.razor"),
                StorySourceRegistry.CodeTab(
                    "default-data",
                    "ExampleSampleData.cs",
                    SampleDataSource,
                    "csharp")
            ])
    ];
}
```

## Example File Rules

- Example files should be full working examples, not fragments.
- Prefer a small, realistic use case over a tiny API demonstration.
- The preview and the displayed code should use the same example file.
- Keep examples focused on the component being documented.
- Avoid external services, timers, random values, or network calls.
- Avoid putting large inline sample data in the story file.
- Use stable IDs and labels so E2E tests can find content reliably.
- Do not edit `StorySourceRegistry.generated.cs` by hand.

## Source Keys

`StorySourceRegistry.CodeTab(...)` takes a source key that must match the file path
relative to `src/components/app` using forward slashes:

```csharp
StorySourceRegistry.CodeTab(
    "default-razor",
    "ButtonDefaultExample.razor",
    "Showcode/Examples/Form/Button/ButtonDefaultExample.razor")
```

Use `"csharp"` as the fourth argument for `.cs` files:

```csharp
StorySourceRegistry.CodeTab(
    "default-model",
    "ButtonExampleModel.cs",
    "Showcode/Examples/Form/Button/ButtonExampleModel.cs",
    "csharp")
```

## Regenerate Sources

Run this after adding or changing files under `Showcode/Examples`:

```powershell
pwsh -File scripts/update-showcode-sources.ps1
```

The app project also runs this generator during normal Windows builds, but running
it explicitly makes the generated diff visible before committing.

## Verify

At minimum:

```powershell
dotnet build src/components/app/LowlandTech.Components.App.csproj -p:SkipTailwindBuild=true
```

When the conversion changes expected story behavior or source-backed tabs, also
run the relevant component E2E tests:

```powershell
dotnet test src/components/e2e/LowlandTech.Components.E2E.csproj --filter "FullyQualifiedName~SPEC1110"
```

If Visual Studio is running the showcase app, it may lock assemblies in
`bin/Debug`. Stop the running app before building.

## Commit Scope

Keep each conversion small:

- One component story per commit is ideal.
- Include generated `StorySourceRegistry.generated.cs` when source files changed.
- Include any `_Imports.razor` change needed for the new examples namespace.
- Reference the tracking issue in the commit message.

Suggested commit shape:

```text
feat(components): convert Button StoryCard to real examples (#139)
```
