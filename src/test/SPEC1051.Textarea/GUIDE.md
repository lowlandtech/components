# Textarea Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1051 |
| **Namespace** | `LowlandTech.Components.Textarea` |
| **Root Element** | `<textarea>` |
| **data-slot** | `textarea` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/textarea |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |
| `placeholder` | `string?` | `null` | Component-specific |
| `rows` | `string?` | `null` | Component-specific |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1051.Textarea/
├── Textarea.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderTextarea.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using TextareaSut = LowlandTech.Components.Textarea.Textarea;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1051")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Textarea component")]
public class SC01_RenderTextarea : ViewScenario
{
    [Fact(DisplayName = "UAC01: Textarea renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TextareaSut>();
        cut.Find("[data-slot='textarea']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Textarea.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
