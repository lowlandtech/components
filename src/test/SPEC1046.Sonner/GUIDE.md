# Sonner Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1046 |
| **Namespace** | `LowlandTech.Components.Sonner` |
| **Root Element** | `<div>` |
| **data-slot** | `sonner-toaster` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/sonner |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1046.Sonner/
├── Sonner.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderSonner.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using SonnerSut = LowlandTech.Components.Sonner.Sonner;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1046")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Sonner component")]
public class SC01_RenderSonner : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sonner renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SonnerSut>();
        cut.Find("[data-slot='sonner-toaster']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Sonner.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
