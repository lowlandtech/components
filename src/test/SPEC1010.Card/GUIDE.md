# Card Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1010 |
| **Namespace** | `LowlandTech.Components.Card` |
| **Root Element** | `<div>` |
| **data-slot** | `card` |
| **Base Class** | `TinyComponentBase` (provides Class, ChildContent, AdditionalAttributes) |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/card |

## Parameters

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Size` | `string?` | `null` | `Card.shared.cs` |
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |

## Sub-Components

| Component | Element | data-slot | Default CSS Class | Extra Parameters |
|-----------|---------|-----------|-------------------|------------------|
| `CardHeader` | `<div>` | `card-header` | `card-header` | — |
| `CardTitle` | `<h3>` | `card-title` | `card-title` | — |
| `CardDescription` | `<p>` | `card-description` | `card-description` | — |
| `CardContent` | `<div>` | `card-content` | `card-content` | — |
| `CardFooter` | `<div>` | `card-footer` | `card-footer` | — |
| `CardAction` | `<div>` | `card-action` | `card-action` | — |

## Nesting Hierarchy

```
Card                          ← <div data-slot="card" data-size class="card">
├── CardHeader                ← <div data-slot="card-header" class="card-header"> (relative positioned)
│   ├── CardTitle             ← <h3 data-slot="card-title" class="card-title">
│   ├── CardDescription       ← <p data-slot="card-description" class="card-description">
│   └── CardAction            ← <div data-slot="card-action" class="card-action"> (absolute top-right)
├── CardContent               ← <div data-slot="card-content" class="card-content">
└── CardFooter                ← <div data-slot="card-footer" class="card-footer">
```

## CSS Classes (from input.css)

```css
.card { @apply rounded-xl border border-border bg-card text-card-foreground shadow-sm; }
.card-header { @apply relative flex flex-col space-y-1.5 p-6; }
.card-title { @apply font-semibold leading-none tracking-tight; }
.card-description { @apply text-sm text-muted-foreground; }
.card-content { @apply p-6 pt-0; }
.card-footer { @apply flex items-center p-6 pt-0; }
.card-action { @apply absolute top-4 right-4; }
```

## Test File Structure

```
SPEC1010.Card/
├── Card.feature                          ← Gherkin spec (this companion)
├── GUIDE.md                              ← This file
└── US01.View/
    ├── SC01_RenderCard.cs                ← Card root tests (UAC01-05)
    ├── SC02_RenderCardHeader.cs          ← CardHeader tests (UAC01-04)
    ├── SC03_RenderCardTitle.cs           ← CardTitle tests (UAC01-04)
    ├── SC04_RenderCardDescription.cs     ← CardDescription tests (UAC01-04)
    ├── SC05_RenderCardContent.cs         ← CardContent tests (UAC01-04)
    ├── SC06_RenderCardFooter.cs          ← CardFooter tests (UAC01-04)
    ├── SC07_RenderCardAction.cs          ← CardAction tests (UAC01-04)
    └── SC08_CardComposition.cs           ← Composition tests (UAC01-02)
```

## Test Patterns

### Basic render test (for each sub-component):
```csharp
using Bunit;
using LowlandTech.Components.Card;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Card component")]
public class SC02_RenderCardHeader : ViewScenario
{
    [Fact(DisplayName = "UAC01: CardHeader renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CardHeader>();
        cut.Find("[data-slot='card-header']").ShouldNotBeNull();
    }
}
```

### Composition test:
```csharp
var cut = Ctx.Render<CardSut>(p => p.AddChildContent(builder =>
{
    builder.OpenComponent<CardHeader>(0);
    builder.AddAttribute(1, "ChildContent", (RenderFragment)(b => { ... }));
    builder.CloseComponent();
}));
var card = cut.Find("[data-slot='card']");
card.QuerySelector("[data-slot='card-header']").ShouldNotBeNull();
```

## Implementation Notes

- Card root is **auto-generated** (`Card.razor` + `Card.razor.cs`) — do not edit directly
- Custom logic goes in `Card.shared.cs` (partial class)
- Sub-components are **hand-crafted** — safe to edit directly
- All sub-components inherit from `TinyComponentBase`
- CSS class merging: default class + `@Class` parameter (e.g., `class="card-header @Class"`)
