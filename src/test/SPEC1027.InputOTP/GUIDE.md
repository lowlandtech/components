# InputOTP Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1027 |
| **Namespace** | `LowlandTech.Components.InputOTP` |
| **Root Element** | `<div>` |
| **data-slot** | `input-otp` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/input-otp |

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
SPEC1027.InputOTP/
├── InputOTP.feature
├── GUIDE.md
└── US01.View/
    └── SC01_RenderInputOTP.cs    ← Basic render tests
```

## Test Pattern

```csharp
using Bunit;
using InputOTPSut = LowlandTech.Components.InputOTP.InputOTP;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1027")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render InputOTP component")]
public class SC01_RenderInputOTP : ViewScenario
{
    [Fact(DisplayName = "UAC01: InputOTP renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<InputOTPSut>();
        cut.Find("[data-slot='input-otp']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- This is an **auto-generated** component — do not edit `.razor` or `.razor.cs` directly
- Put custom logic in `InputOTP.shared.cs` (partial class)
- Currently a stub — needs sub-components and behavior per shadcn spec
