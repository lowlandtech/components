# Accordion Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1000 |
| **Namespace** | `LowlandTech.Components.Accordion` |
| **Root Element** | `<div>` |
| **data-slot** | `accordion` |
| **Base Class** | `LtComponentBase` |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/accordion |

## Parameters

### Accordion (root)

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `LtComponentBase` |
| `Type` | `string` | `"single"` | `Accordion.shared.cs` |
| `Collapsible` | `bool` | `false` | `Accordion.shared.cs` |
| `DefaultValue` | `string?` | `null` | `Accordion.shared.cs` |

### AccordionItem

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Value` | `string?` | `null` | `AccordionItem.razor.cs` |
| `Disabled` | `bool` | `false` | `AccordionItem.razor.cs` |
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |

### AccordionTrigger

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |

### AccordionContent

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Class` | `string?` | `null` | `LtComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `LtComponentBase` |

## Sub-Components

| Component | Element | data-slot | Purpose |
|-----------|---------|-----------|---------|
| `Accordion` | `<div>` | `accordion` | Root container, owns AccordionState |
| `AccordionItem` | `<div>` | `accordion-item` | Wraps one collapsible section |
| `AccordionTrigger` | `<button>` | `accordion-trigger` | Clickable header that toggles content |
| `AccordionContent` | `<div>` | `accordion-content` | Collapsible body (conditionally rendered) |

## Nesting Hierarchy

```
Accordion                          ← CascadingValue<AccordionState>
├── AccordionItem                  ← CascadingValue<AccordionItemContext>
│   ├── AccordionTrigger           ← <button> calls Toggle on click
│   └── AccordionContent           ← conditionally rendered when open
├── AccordionItem
│   ├── AccordionTrigger
│   └── AccordionContent
└── ...
```

## State Management

### AccordionState

Manages which items are open. Created by the root `Accordion` component and cascaded to children.

| Property/Method | Type | Description |
|----------------|------|-------------|
| `Type` | `string` | `"single"` or `"multiple"` |
| `Collapsible` | `bool` | Whether open items can be closed by clicking again |
| `IsOpen(value)` | `bool` | Check if a specific item is open |
| `Toggle(value)` | `void` | Toggle an item open/closed |
| `SetOpen(value)` | `void` | Force an item open |
| `OpenItems` | `IReadOnlySet<string>` | Currently open item values |
| `OnChange` | `event Action` | Fires when state changes |

### AccordionItemContext

Cascaded from `AccordionItem` to `AccordionTrigger` and `AccordionContent`.

| Property | Type | Description |
|----------|------|-------------|
| `Value` | `string?` | The item's unique value |
| `Disabled` | `bool` | Whether the item is disabled |
| `IsOpen` | `bool` | Whether this item is currently open |
| `AccordionState` | `AccordionState?` | Reference to parent state |

## Data Attributes

| Attribute | Component | Values | Description |
|-----------|-----------|--------|-------------|
| `data-slot` | All | Component name | CSS hook for styling |
| `data-type` | Accordion | `"single"`, `"multiple"` | Expansion mode |
| `data-state` | AccordionItem, AccordionTrigger, AccordionContent | `"open"`, `"closed"` | Current open/closed state |
| `data-disabled` | AccordionItem | `"true"` or absent | Whether item is disabled |

## Behavior Rules

1. **Single mode (default)**: Only one item can be open at a time. Opening a new item closes the previous one.
2. **Multiple mode**: Any number of items can be open simultaneously.
3. **Collapsible**: When `true`, clicking an open item's trigger closes it. When `false` (default in single mode), the open item cannot be closed by clicking its trigger.
4. **Disabled items**: A disabled item's trigger click is ignored. The item renders `data-disabled="true"`.
5. **DefaultValue**: Pre-opens a specific item on initial render.
6. **Content rendering**: `AccordionContent` is conditionally rendered — not in the DOM when closed.

## Test File Structure

```
SPEC1000.Accordion/
├── Accordion.feature              ← 35 Gherkin scenarios
├── GUIDE.md                       ← This file
├── US01.View/
│   ├── SC01_RenderAccordion.cs    ← Root render tests (6 UACs)
│   ├── SC02_RenderAccordionItem.cs ← Item render tests (5 UACs)
│   ├── SC03_RenderAccordionTrigger.cs ← Trigger render tests (5 UACs)
│   ├── SC04_RenderAccordionContent.cs ← Content render tests (4 UACs)
│   └── SC05_AccordionBehavior.cs  ← Interactive behavior tests (6 UACs)
└── US02.State/
    └── SC01_AccordionStateManagement.cs ← Pure state logic tests (9 UACs)
```

## Test Patterns

### Rendering an Accordion with items (C# RenderTreeBuilder)

```csharp
var cut = Ctx.Render<AccordionSut>(p => p
    .Add(x => x.Type, "single")
    .Add(x => x.Collapsible, true)
    .AddChildContent(builder =>
    {
        builder.OpenComponent<AccordionItem>(0);
        builder.AddAttribute(1, "Value", "item-1");
        builder.AddAttribute(2, "ChildContent",
            (RenderFragment)(b =>
            {
                b.OpenComponent<AccordionTrigger>(0);
                b.AddAttribute(1, "ChildContent",
                    (RenderFragment)(b2 => b2.AddContent(0, "Trigger 1")));
                b.CloseComponent();
                b.OpenComponent<AccordionContent>(0);
                b.AddAttribute(1, "ChildContent",
                    (RenderFragment)(b2 => b2.AddContent(0, "Content 1")));
                b.CloseComponent();
            }));
        builder.CloseComponent();
    }));
```

### Testing state directly (no rendering)

```csharp
var state = new AccordionState { Type = "single", Collapsible = true };
state.Toggle("item-1");
state.IsOpen("item-1").ShouldBeTrue();
state.Toggle("item-2");
state.IsOpen("item-1").ShouldBeFalse();
state.IsOpen("item-2").ShouldBeTrue();
```

## Implementation Notes

- The `Accordion.razor` file is hand-crafted (not auto-generated) because it wraps children in a `CascadingValue<AccordionState>` and renders `data-type`
- Custom logic lives in `Accordion.shared.cs` (partial class with `Type`, `Collapsible`, `DefaultValue` parameters)
- `AccordionContent` uses conditional rendering (`@if`) — content is not in the DOM when closed
- `AccordionTrigger` respects `Disabled` via `AccordionItemContext` — disabled triggers ignore clicks
