# Alert Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1001 |
| **Namespace** | `LowlandTech.Components.Alert` |
| **Root Element** | `<div>` |
| **data-slot** | `alert` |
| **Base Class** | `TinyComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/alert |

## Parameters

### Alert (root)

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string?` | `"default"` | Visual variant: `"default"` or `"destructive"` |
| `Class` | `string?` | `null` | Additional CSS classes (merged with `alert` base class) |
| `ChildContent` | `RenderFragment?` | `null` | Child content (AlertTitle, AlertDescription, AlertAction, icons) |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | Splatted onto root element |

### AlertTitle

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` | `null` | Additional CSS classes (merged with `alert-title` base class) |
| `ChildContent` | `RenderFragment?` | `null` | Title text or markup (supports `<a>` links) |

### AlertDescription

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` | `null` | Additional CSS classes (merged with `alert-description` base class) |
| `ChildContent` | `RenderFragment?` | `null` | Description text or rich HTML (supports `<p>`, `<ul>`, `<a>`) |

### AlertAction

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` | `null` | Additional CSS classes (merged with `alert-action` base class) |
| `ChildContent` | `RenderFragment?` | `null` | Interactive content (buttons, badges) |

## Sub-Components

| Component | data-slot | Element | Base CSS Class |
|-----------|-----------|---------|---------------|
| `Alert` | `alert` | `<div>` | `alert` |
| `AlertTitle` | `alert-title` | `<div>` | `alert-title` |
| `AlertDescription` | `alert-description` | `<div>` | `alert-description` |
| `AlertAction` | `alert-action` | `<div>` | `alert-action` |

## Rendered Markup

```html
<div data-slot="alert" role="alert" data-variant="default" class="alert {Class}">
    <svg>...</svg>                          <!-- optional icon -->
    <div data-slot="alert-title" class="alert-title">...</div>
    <div data-slot="alert-description" class="alert-description">...</div>
    <div data-slot="alert-action" class="alert-action">...</div>
</div>
```

## Variants

| Variant | data-variant | Usage |
|---------|-------------|-------|
| `default` | `"default"` | Informational alerts (default when omitted) |
| `destructive` | `"destructive"` | Error / danger alerts |

## Composition Patterns (from shadcn examples)

1. **Title only** — `Alert > AlertTitle`
2. **Description only** — `Alert > AlertDescription`
3. **Title + Description** — `Alert > AlertTitle + AlertDescription`
4. **Icon + Title** — `Alert > svg + AlertTitle`
5. **Icon + Description** — `Alert > svg + AlertDescription`
6. **Icon + Title + Description** — `Alert > svg + AlertTitle + AlertDescription`
7. **Icon + Title + Action** — `Alert > svg + AlertTitle + AlertAction`
8. **Icon + Title + Description + Action** — `Alert > svg + AlertTitle + AlertDescription + AlertAction`
9. **Title + Description + Action** — `Alert > AlertTitle + AlertDescription + AlertAction`

## Test File Structure

```
SPEC1001.Alert/
├── GUIDE.md
└── US01.View/
    ├── US01.View.feature             ← 20 scenarios, 48 UACs
    └── SC01_RenderAlert.cs           ← Test implementations
```

## Scenario Summary (US01.View)

| Scenario | UACs | Focus |
|----------|------|-------|
| SC01 | UAC01–05 | Alert root defaults: data-slot, div, role, class, variant |
| SC02 | UAC06–08 | Alert parameters: destructive variant, Class merge |
| SC03 | UAC09 | Alert child content |
| SC04 | UAC10 | Alert additional attributes passthrough |
| SC05 | UAC11–12 | AlertTitle defaults: data-slot, class |
| SC06 | UAC13–15 | AlertTitle content and Class merge |
| SC07 | UAC16 | AlertTitle with anchor links |
| SC08 | UAC17–18 | AlertDescription defaults: data-slot, class |
| SC09 | UAC19–21 | AlertDescription content and Class merge |
| SC10 | UAC22–24 | AlertDescription rich HTML: paragraph, list, links |
| SC11 | UAC25–26 | AlertAction defaults: data-slot, class |
| SC12 | UAC27–29 | AlertAction content and Class merge |
| SC13 | UAC30–31 | AlertAction with interactive button |
| SC14 | UAC32–33 | Compose title + description + action |
| SC15 | UAC34–36 | Compose destructive variant with sub-components |
| SC16 | UAC37–38 | Compose title only (no description) |
| SC17 | UAC39–40 | Compose description only (no title) |
| SC18 | UAC41 | Compose with SVG icon child |
| SC19 | UAC42–44 | Compose icon + title + description |
| SC20 | UAC45–48 | Compose icon + title + action (no description) |

## Test Pattern

```csharp
using Bunit;
using AlertSut = LowlandTech.Components.Alert.Alert;
using AlertTitleSut = LowlandTech.Components.Alert.AlertTitle;
using AlertDescriptionSut = LowlandTech.Components.Alert.AlertDescription;
using AlertActionSut = LowlandTech.Components.Alert.AlertAction;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1001")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Alert component")]
public class SC01_RenderAlert : ViewScenario
{
    [Fact(DisplayName = "UAC01: Alert renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AlertSut>();
        cut.Find("[data-slot='alert']").ShouldNotBeNull();
    }
}
```

## Implementation Notes

- Alert root renders `role="alert"` for accessibility
- Alert root renders `data-variant` attribute (defaults to `"default"`)
- All sub-components are `<div>` elements with `data-slot` attributes
- Sub-components merge `Class` parameter with their base CSS class
- Icons are passed as direct children of Alert (SVG elements), not via a parameter
- AlertDescription supports arbitrary HTML content (paragraphs, lists, links)
- AlertAction is positioned absolutely (CSS concern, not tested in bUnit)
- Additional attributes are splatted onto the root element via `@attributes`
