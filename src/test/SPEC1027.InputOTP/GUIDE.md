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

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` | `null` | Additional CSS classes for the root container. From `TinyComponentBase`. |
| `ChildContent` | `RenderFragment?` | `null` | Child content. From `TinyComponentBase`. |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | Additional attributes for the root container. From `TinyComponentBase`. |
| `Length` | `int` | `6` | Number of OTP input boxes. |
| `Value` | `string` | `""` | Current OTP value. |
| `ValueChanged` | `EventCallback<string>` | — | Fires when the value changes. |
| `Completed` | `EventCallback<string>` | — | Fires when the last digit is entered and the OTP is complete. |
| `Disabled` | `bool` | `false` | Disables all inputs. |
| `Mask` | `bool` | `false` | Renders inputs as `type="password"` and masks entered digits. |
| `MaskChar` | `char` | `'•'` | Character used to mask entered digits when `Mask` is true. |

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

## Behavior

- Only a single digit (`0-9`) is accepted in each input box.
- When a digit is entered, focus automatically advances to the next input via JS interop.
- Pressing `Backspace` in an empty box clears the previous box and moves focus back.
- When the last digit is entered, the `Completed` callback fires with the full value.
- When `Mask="true"`, inputs render as `type="password"` and entered digits are replaced by `MaskChar`.

## Implementation Notes

- Implementation lives in `InputOTP.razor`.
- Focus navigation is handled by `wwwroot/js/input-otp.js`.
