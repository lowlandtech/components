# Avatar Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1004 |
| **Namespace** | `LowlandTech.Components.Avatar` |
| **Root Element** | `<span>` |
| **data-slot** | `avatar` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/avatar |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `LtComponentBase` |

## Sub-Components

None (auto-generated stub). See shadcn reference for full sub-component API.

## Test File Structure

```
SPEC1004.Avatar/
├── Avatar.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderAvatar.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using AvatarSut = LowlandTech.Components.Avatar.Avatar;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1004")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Avatar component")]
public class SC01_RenderAvatar : ViewScenario
{
    [Fact(DisplayName = "UAC01: Avatar renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AvatarSut>();
        cut.Find("[data-slot='avatar']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `Avatar.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
