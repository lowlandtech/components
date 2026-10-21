# Theming Architecture Plan

## Purpose

LowlandTech.Components currently has a simple but useful scoped token system: `ThemeProvider` selects an `IComponentTheme`, injects CSS custom properties into a provider boundary, and child components consume those variables through Tailwind/shadcn-style classes.

That is the right foundation, but it is not enough for themes that need a different visual language. Glassmorphism, corporate grey, chadcn black, dense enterprise, playful consumer, and plugin-branded surfaces need more than color tokens. They need controlled ways to override component class slots and, in rare cases, replace component fragments.

This plan evolves theming from:

```text
theme = CSS custom properties
```

to:

```text
theme = tokens + class slots + component fragments
```

The goal is full restyling without forking components or copying entire pages.

## Current State

The current system lives in:

- `src/components/lib/Theme/IComponentTheme.cs`
- `src/components/lib/Theme/ComponentThemeService.cs`
- `src/components/lib/Theme/ThemeProviderContext.cs`
- `src/components/lib/ThemeProvider/ThemeProvider.razor`
- `src/components/lib/ThemeToggle/ThemeToggle.razor`

Current behavior:

- `ThemeProvider` creates a scoped boundary using `data-theme-provider="{Id}"`.
- `ComponentThemeService` tracks the active theme name per provider.
- `IComponentTheme.Tokens` emits CSS custom-property overrides such as `--background`, `--primary`, `--border`.
- Child components inherit the CSS variables.
- Nested providers and sibling providers can have independent active themes.

This supports palette changes well. It does not yet support structural style changes well.

Example limitation:

- `Button` hard-codes `"btn"`, variant classes, size classes, and consumer `Class`.
- `Card` hard-codes `"cn-card card group/card"` and elevation classes.
- A glass theme can change `--card`, but cannot centrally change every card to use backdrop blur, translucent borders, different elevation rules, or alternate nested slot styling.

## Implementation Review Notes

The first implementation now has the right shape: `ComponentThemeService` implements a resolver, `ThemeProviderContext` cascades the active provider and resolver, `ThemeProvider` emits `data-theme-name` and `data-theme-style`, and `Button`, `Badge`, and `Card` can resolve `ThemeClass` slot overrides.

Before this is treated as complete, tighten the implementation and tests around the following gaps.

### Contract Test Coverage

The shipped theme contract tests must include every concrete theme registered by the component stories/app, not only the earlier token examples.

Required additions:

- Add `ChadcnBlackComponentTheme`, `CorporateGreyComponentTheme`, and `GlassComponentTheme` to the contract test matrix.
- Replace hard-coded assumptions such as `--primary-foreground == "#ffffff"` with an actual semantic check. `ChadcnBlack` correctly uses a dark primary foreground on a light primary token, so white-only assertions are not a valid contrast guarantee.
- Keep the required semantic token set, but allow extra tokens such as `--radius`, `--surface-blur`, `--control-height-md`, and density/motion tokens.
- Add a token safety check: token keys must start with `--`, token keys must not contain braces/semicolons, and token values must not break the scoped style block.
- Add a registration-order test that verifies the real story/app DI order, because `ThemeToggle` cycling depends on that order.

### Resolver Behavior Tests

`ComponentThemeService.Class` currently resolves exact slot keys and a component+slot wildcard. That is a reasonable rule, but it must be explicit and protected by tests.

Required tests:

- exact match wins over wildcard
- wildcard applies when variant/size/state do not match
- no partial fallback occurs unless intentionally added later
- missing provider falls back to component defaults
- unknown theme name renders child content and does not throw
- `Append`, `Prepend`, and `Replace` preserve consumer `Class`
- `Replace` still preserves design-mode classes, disabled classes, focus obligations, and caller layout classes
- nested providers resolve different class slots simultaneously
- sibling providers do not leak class slots into each other

If partial fallback is desired later, define the order before implementing it. For example:

1. component + slot + variant + size + state
2. component + slot + variant + size
3. component + slot + variant
4. component + slot

Do not let each component invent its own fallback chain.

### Fragment System Is Not Proven Yet

The fragment contracts exist, but no converted component currently renders a theme fragment. That means the most powerful part of the architecture is contract-only.

Required next step:

- Either mark fragments as deferred in code comments and docs, or add one tiny fragment slot such as `Card.Decoration`.
- Add bUnit coverage that renders the fragment inside a provider, passes a `ThemeFragmentContext`, and falls back cleanly without a provider.
- Add a negative test proving a fragment cannot replace keyboard or ARIA ownership for an interactive component root.

Until at least one fragment slot is rendered by a real component and tested, do not claim "component fragments" are implemented.

### Converted Component Coverage

Only `Button`, `Badge`, and `Card` are currently in the class-slot path. Each converted component needs a small but strict test matrix.

For every converted component:

- renders default classes with no provider
- applies exact class slot under a provider
- applies wildcard class slot when no exact slot exists
- preserves consumer `Class`
- preserves design-mode classes
- preserves disabled/focus/ARIA/data attributes
- supports nested provider overrides
- documents supported slot names in the component doc

The first deeper conversions should be `Input`, `Dialog`, and `Tabs` because they will expose whether the slot model survives compound components, focus management, and nested interactive markup.

### Visual And Accessibility Gates

The new packs are visual-language themes, so unit tests are not enough.

Required gates before calling a pack complete:

- Story matrix renders `Button`, `Badge`, `Card`, `Input`, `Dialog`, `Tabs`, and at least one dense data surface under `ChadcnBlack`, `CorporateGrey`, and `Glass`.
- Screenshot tests capture each theme pack at desktop and mobile widths.
- Glass screenshots must include a non-flat background so translucency and blur can be inspected.
- Focus-visible, disabled, hover, active, selected, and invalid states are captured for every interactive converted component.
- Add accessibility assertions for name/role preservation and keyboard behavior on Dialog/Tabs after class replacement.
- Add contrast checks for token pairs instead of fixed color assertions.

### CSS And Tailwind Safety

Class slots currently contain static strings in C# theme classes, which is acceptable only if the build can see or safelist those classes.

Required checks:

- Verify Tailwind output contains slot-only classes used by theme packs, especially `backdrop-blur-*`, opacity modifiers, and theme-specific radius/density classes.
- If class slots can later come from plugins or runtime-loaded packages, move required theme slot classes into a stable CSS layer or safelist them explicitly.
- Add a generated CSS smoke test or visual story check so a theme cannot pass bUnit while silently losing its Tailwind classes.

### Definition Of Done For The Current Implementation

The current implementation should be considered complete only when:

- all registered theme packs are in contract tests
- `Button`, `Badge`, and `Card` have resolver-backed bUnit tests
- at least one fragment slot is either implemented and tested or explicitly deferred
- provider tests assert `data-theme-name`, `data-theme-style`, token CSS, nested independence, and cleanup
- visual tests prove the three new style families render differently from token-only themes
- accessibility tests prove class replacement did not remove focus, disabled, role, ARIA, or keyboard behavior

## Design Goals

- Keep simple token themes simple.
- Allow advanced themes to restyle components without duplicating components.
- Preserve component APIs and bUnit tests.
- Preserve accessibility, disabled states, focus rings, and design-mode behavior.
- Keep themes scoped to `ThemeProvider` boundaries.
- Support multiple active themes at once.
- Avoid runtime string soup inside components.
- Make missing slot overrides harmless.
- Allow plugins to register theme packs.
- Keep MAUI/Blazor Hybrid safe.

## Non-Goals

- Do not turn every component into arbitrary template soup.
- Do not replace CSS variables with C# styling.
- Do not require every theme to define every component.
- Do not create a separate component library per theme.
- Do not make glassmorphism the default. It should be one theme pack beside others.

## Architecture Overview

Use three layers, ordered from broadest to most specific.

### Layer 1: Design Tokens

Tokens remain CSS custom properties.

They own:

- color
- radius
- shadow
- typography families
- chart colors
- sidebar tokens
- motion duration/easing tokens
- density tokens, if added
- surface effect tokens such as blur and opacity

### Layer 2: Class Slots

Class slots let a theme override named class groups for a component.

Examples:

- `Button.Root`
- `Button.Icon`
- `Card.Root`
- `Card.Header`
- `Card.Content`
- `Dialog.Overlay`
- `Dialog.Content`
- `DataGrid.Row`
- `Tabs.List`
- `Tabs.Trigger`

Class slots are for structural styling that still uses the same markup.

### Layer 3: Component Fragments

Fragments let a theme replace a small named render area when class slots are not enough.

Examples:

- `Card.Decoration`
- `Dialog.Backdrop`
- `Empty.IconFrame`
- `Tabs.ActiveIndicator`
- `DataGrid.EmptyState`

Fragments must be rare and named. They should not replace entire components by default.

## Proposed Contracts

Extend `IComponentTheme` conservatively.

```csharp
public interface IComponentTheme
{
    string Name { get; }
    IReadOnlyDictionary<string, string> Tokens { get; }
}
```

Add optional capability interfaces so existing themes keep compiling:

```csharp
public interface IComponentClassTheme
{
    IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; }
}

public interface IComponentFragmentTheme
{
    IReadOnlyDictionary<ComponentSlotKey, RenderFragment<ThemeFragmentContext>> Fragments { get; }
}
```

Slot keys should be structured, not loose strings:

```csharp
public readonly record struct ComponentSlotKey(
    string Component,
    string Slot,
    string? Variant = null,
    string? Size = null,
    string? State = null);
```

Class slot values should describe merge behavior:

```csharp
public sealed record ThemeClassSlot(
    string Class,
    ThemeClassMergeMode MergeMode = ThemeClassMergeMode.Append);

public enum ThemeClassMergeMode
{
    Append,
    Replace,
    Prepend
}
```

Fragment context should be small and stable:

```csharp
public sealed record ThemeFragmentContext(
    string Component,
    string Slot,
    string? Variant,
    string? Size,
    IReadOnlyDictionary<string, object?> Parameters);
```

## Theme Resolution Service

Add a resolver beside `ComponentThemeService`.

```csharp
public interface IComponentThemeResolver
{
    string Class(
        string providerId,
        ComponentSlotKey key,
        string defaultClass,
        string? consumerClass = null);

    RenderFragment<ThemeFragmentContext>? Fragment(
        string providerId,
        ComponentSlotKey key);
}
```

`ComponentThemeService` can implement this directly, or delegate to a `ComponentThemeResolver`.

Resolution order:

1. component default class
2. active theme class slot, if registered
3. consumer `Class`
4. design-mode classes

Consumer `Class` should normally remain last so local composition can still add layout constraints.

## Cascading Theme Context

Expand `ThemeProviderContext` so components can resolve theme slots without each needing to inject services.

Current context:

```csharp
public sealed class ThemeProviderContext
{
    public string ProviderId { get; }
    public ComponentThemeService ThemeService { get; }
}
```

Target context:

```csharp
public sealed class ThemeProviderContext
{
    public string ProviderId { get; }
    public IComponentThemeResolver Resolver { get; }
    public IComponentTheme? ActiveTheme { get; }
}
```

Components should accept this as an optional cascading parameter. If no provider is present, they use defaults.

## Component Authoring Pattern

Components should stop hand-joining themeable classes directly in markup.

Target pattern:

```csharp
private string RootClass => Theme.Class(
    new("Button", "Root", Variant, Size, State),
    defaultClass: ButtonDefaults.Root(Variant, Size, State),
    consumerClass: Class,
    designClass: DesignModeClass);
```

For practical implementation, this can be a helper on `TinyComponentBase` or `TinyComponentBase`:

```csharp
protected string ThemeClass(
    string component,
    string slot,
    string defaultClass,
    string? variant = null,
    string? size = null,
    string? state = null,
    string? consumerClass = null,
    string? designClass = null)
```

Then `Button` becomes:

```csharp
private string GetClass() => ThemeClass(
    "Button",
    "Root",
    ButtonDefaults.Root(Variant, Size, EffectiveState),
    variant: Variant,
    size: Size,
    state: EffectiveState,
    consumerClass: Class,
    designClass: $"{DesignModeClass} {DesignDisabledClass}");
```

## Token Expansion

Add tokens for effects and density before building glass or corporate themes.

Suggested additions:

```css
--surface-opacity: 1;
--surface-blur: 0px;
--surface-saturation: 1;
--surface-border-opacity: 1;
--surface-shadow-opacity: 0.10;
--control-height-sm: 2rem;
--control-height-md: 2.25rem;
--control-height-lg: 2.5rem;
--control-padding-x: 1rem;
--density-scale: 1;
--motion-duration-fast: 120ms;
--motion-duration-normal: 180ms;
--motion-ease-standard: cubic-bezier(.2, 0, 0, 1);
```

These keep many theme differences in CSS variables rather than class overrides.

## Theme Packs

Register themes as packs. A pack can include tokens only, tokens plus class slots, or tokens plus class slots and fragments.

Initial packs:

- `ChadcnBlack`: close to shadcn neutral black/white defaults.
- `CorporateGrey`: restrained enterprise neutral palette, tighter radius, minimal shadow.
- `Glass`: translucent surfaces, backdrop blur, softer borders, more layered elevation.
- `FoundryDefault`: current LowlandTech purple/orange baseline.
- `Ocean`, `Rose`, `Forest`: keep as simple scoped token examples.

Example:

```csharp
public sealed class GlassComponentTheme :
    IComponentTheme,
    IComponentClassTheme,
    IComponentFragmentTheme
{
    public string Name => "Glass";

    public IReadOnlyDictionary<string, string> Tokens { get; } = new Dictionary<string, string>
    {
        ["--background"] = "oklch(0.18 0.03 260)",
        ["--foreground"] = "oklch(0.96 0.01 260)",
        ["--card"] = "oklch(0.28 0.03 260 / 0.58)",
        ["--border"] = "oklch(0.95 0.01 260 / 0.18)",
        ["--surface-blur"] = "18px",
        ["--surface-opacity"] = "0.62"
    };

    public IReadOnlyDictionary<ComponentSlotKey, ThemeClassSlot> ClassSlots { get; } =
        new Dictionary<ComponentSlotKey, ThemeClassSlot>
        {
            [new("Card", "Root")] = new(
                "bg-card/70 backdrop-blur-xl border-white/15 shadow-2xl",
                ThemeClassMergeMode.Replace),

            [new("Button", "Root", Variant: "default")] = new(
                "bg-primary/80 backdrop-blur-md hover:bg-primary border border-white/10",
                ThemeClassMergeMode.Append)
        };
}
```

## Fragment Guidelines

Fragments are powerful and dangerous. Use them only when the same semantic component needs a real visual part that does not exist in the default markup.

Good fragment slots:

- decorative active indicator
- empty state illustration frame
- card background layer
- dialog backdrop treatment
- command palette leading icon treatment

Bad fragment slots:

- entire button root
- entire data grid
- entire dialog content
- arbitrary page content

Rule: if replacing a fragment would break keyboard behavior or ARIA ownership, it is too large.

## CSS Strategy

Create a dedicated theme CSS layer.

Suggested files:

```text
src/components/lib/wwwroot/css/
├── variables.css
├── theme-slots.css
└── input.css
```

`theme-slots.css` should contain stable low-level classes and data attributes, not theme-specific one-off page rules.

Examples:

```css
[data-theme-style="glass"] [data-slot="card"] {
    background: color-mix(in oklab, var(--card) 72%, transparent);
    backdrop-filter: blur(var(--surface-blur));
}
```

This can complement C# class slots. Use CSS when a theme can be expressed by provider attributes and tokens; use C# class slots when component variants or state need targeted class merging.

## ThemeProvider Changes

`ThemeProvider` should emit:

- `data-theme-provider`
- `data-theme-name`
- `data-theme-style`, optional
- scoped token style block
- cascading `ThemeProviderContext`

Example:

```razor
<div data-theme-provider="@Id"
     data-theme-name="@ActiveTheme?.Name"
     data-theme-style="@ActiveThemeStyle"
     class="@Class">
```

`data-theme-style` can group themes by treatment:

- `default`
- `neutral`
- `glass`
- `high-contrast`
- `brand`

## Migration Plan

### Phase 1: Document Slot Names

For every component, list supported slots in its doc.

Start with:

- Button
- Card
- Dialog
- Sheet
- Tabs
- Input
- Select
- DataGrid
- Sidebar
- Table
- Badge
- Alert

Deliverables:

- slot naming convention
- `ComponentSlotKey`
- docs examples

### Phase 2: Resolver And Context

Add the resolver contract and cascade it through `ThemeProvider`.

Deliverables:

- optional class-slot interface
- optional fragment interface
- resolver tests
- provider boundary tests
- nested provider tests

### Phase 3: Convert Core Components

Convert a small vertical slice first.

Recommended order:

1. `Button`
2. `Card`
3. `Badge`
4. `Input`
5. `Dialog`
6. `Tabs`

Deliverables:

- no breaking API changes
- existing tests pass
- new tests prove class-slot override and fallback behavior

### Phase 4: Add Theme Packs

Create real theme packs in stories first.

Deliverables:

- `ChadcnBlackComponentTheme`
- `CorporateGreyComponentTheme`
- `GlassComponentTheme`
- story showing all three side by side
- screenshot/visual review baseline

### Phase 5: Fragment Slots

Add fragment slots only after class slots prove insufficient.

Start with:

- `Card.Decoration`
- `Dialog.Backdrop`
- `Tabs.ActiveIndicator`
- `Empty.IconFrame`

Deliverables:

- fragment resolver tests
- bUnit rendering tests
- accessibility regression check for dialog/sheet

### Phase 6: Plugin Theme Contributions

Allow plugins to contribute theme packs through DI.

Deliverables:

- theme pack registration extension
- plugin-owned theme examples
- tests proving plugin themes appear in `ThemeToggle` cycle
- hot-reload safety review

## Testing Strategy

Unit tests:

- resolver returns default when no provider exists
- resolver appends, prepends, and replaces classes correctly
- consumer `Class` is preserved
- nested providers resolve independently
- missing slot does not throw
- fragment lookup respects provider theme

bUnit tests:

- `Button` applies theme class slot
- `Card` applies theme class slot
- `ThemeProvider` emits data attributes and token CSS
- `ThemeToggle` cycles token and slot themes
- component fallback still matches current class output

Visual tests:

- theme matrix story for core components
- one page-level story for each theme pack
- light/dark/scoped provider examples
- screenshot compare for corporate grey, chadcn black, glass

## Risks

- Too many slot names can make themes brittle.
- Replacing classes can accidentally remove accessibility/focus styles.
- Fragment slots can become component forks if too broad.
- Theme-specific CSS can leak if provider selectors are not scoped.
- Generated CSS can grow if every theme creates many unique Tailwind classes.

Mitigations:

- keep default classes in component code
- require all replacement slots to include focus/disabled obligations
- prefer CSS variables over class slots when possible
- keep fragment slots small and named
- add docs for each supported slot

## Acceptance Criteria

- Existing token themes still work unchanged.
- A token-only theme can be registered with the old `IComponentTheme` shape.
- An advanced theme can override `Button.Root` and `Card.Root` without editing those components.
- A glass theme can render translucent card/dialog surfaces through scoped provider boundaries.
- A corporate grey theme can tighten radius, shadow, and component chrome without per-page CSS.
- A chadcn black theme can closely match neutral shadcn styling.
- Nested providers can run different style families at the same time.
- Components remain accessible and testable.

## Navigation

[[components/_index|Back to Index]]
