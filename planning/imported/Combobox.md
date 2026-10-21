# Combobox (SPEC1015)

**Status:** 🔍 Reviewed with feedback

## Summary

All major subcomponents are ported (Combobox, ComboboxInput, ComboboxContent, ComboboxItem, ComboboxList, ComboboxEmpty, ComboboxGroup, ComboboxLabel, ComboboxSeparator) with internal open/select state management. Key gaps: no client-side item filtering from input text, `OnFocus` toggles rather than opens, `ComboboxItem.Disabled` does not set `data-disabled` attribute, `ComboboxContent` uses `@if` instead of `data-state`, no keyboard navigation, and `IsOpen` state is not externally bindable.

## What Is Ported

- `Combobox` — root with `Value`/`ValueChanged`, internal `IsOpen`/`InputText`, `Toggle`/`Close`/`SelectItem` methods
- `ComboboxInput` — text input with chevron trigger, optional clear button, `Placeholder`, `Disabled`, `ShowTrigger`, `ShowClear`
- `ComboboxContent` — dropdown panel (conditionally rendered when `IsOpen`)
- `ComboboxItem` — selectable item with checkmark indicator, `Value`, `Disabled`
- `ComboboxList` — scrollable list wrapper
- `ComboboxEmpty` — empty state placeholder
- `ComboboxGroup` — item grouping
- `ComboboxLabel` — group label
- `ComboboxSeparator` — visual divider

## Gaps vs shadcn Reference

### 1. No client-side item filtering

`ComboboxInput` writes typed text to `Parent.InputText`, but `ComboboxItem` and `ComboboxList` do not read it. Items are never filtered — all items render regardless of what the user types.

```gherkin
Feature: Combobox Filtering
  As a user
  I want items to filter as I type
  So that I can quickly find the option I need

  Scenario: Items are filtered by input text
    Given a Combobox with items ["Apple", "Banana", "Cherry"]
    When the user types "an"
    Then only "Banana" is visible
    And "Apple" and "Cherry" are hidden

  Scenario: ComboboxEmpty shows when no items match
    Given a Combobox with items ["Apple", "Banana"]
    When the user types "xyz"
    Then no ComboboxItems are visible
    And ComboboxEmpty is visible

  Scenario: All items show when input is cleared
    Given a filtered Combobox
    When the user clears the input
    Then all items are visible again
```

### 2. OnFocus toggles instead of opening

`ComboboxInput.OnFocus` calls `Parent.Toggle()`, which closes an already-open combobox when the user refocuses the input. It should only open.

```gherkin
Feature: Combobox Focus Behaviour
  As a user
  I want the dropdown to open when I focus the input
  So that I do not accidentally close it by clicking the input field

  Scenario: Focusing a closed combobox opens it
    Given a Combobox that is closed
    When the input receives focus
    Then the combobox opens

  Scenario: Focusing an open combobox does not close it
    Given a Combobox that is open
    When the input receives focus
    Then the combobox remains open
```

### 3. ComboboxItem.Disabled does not set data-disabled attribute

`ComboboxItem` accepts a `Disabled` parameter and skips `SelectItem` on click, but does not add `data-disabled` or `aria-disabled` to the rendered element. CSS selectors like `data-disabled:opacity-50` in the class string are never activated.

```gherkin
Feature: ComboboxItem Disabled State
  As a developer
  I want a disabled item to expose a CSS hook
  So that I can style it as visually disabled

  Scenario: Disabled ComboboxItem has data-disabled attribute
    Given a ComboboxItem with Disabled=true
    Then the element has attribute data-disabled

  Scenario: Disabled ComboboxItem has aria-disabled="true"
    Given a ComboboxItem with Disabled=true
    Then the element has aria-disabled="true"

  Scenario: Enabled ComboboxItem does not have data-disabled attribute
    Given a ComboboxItem with Disabled=false
    Then the element does not have attribute data-disabled
```

### 4. ComboboxContent uses @if instead of data-state

The dropdown panel is removed from the DOM when closed, preventing CSS exit animations. It should stay rendered with `data-state="open|closed"`.

```gherkin
Feature: ComboboxContent Animation State
  As a user
  I want the dropdown to animate closed
  So that the dismiss transition is visible

  Scenario: ComboboxContent has data-state=open when open
    Given a Combobox with IsOpen=true
    Then ComboboxContent has attribute data-state="open"
    And it has class data-[state=open]:animate-in

  Scenario: ComboboxContent has data-state=closed when closed
    Given a Combobox with IsOpen=false
    Then ComboboxContent has attribute data-state="closed"
    And the element remains in the DOM
```

### 5. No keyboard navigation

The combobox does not respond to keyboard events for navigating and selecting items.

```gherkin
Feature: Combobox Keyboard Navigation
  As a user
  I want to navigate combobox options with the keyboard
  So that the component is accessible without a mouse

  Scenario: ArrowDown moves focus to the first item
    Given an open Combobox
    When the user presses ArrowDown
    Then the first ComboboxItem receives focus

  Scenario: ArrowDown advances to the next item
    Given an open Combobox with focus on the first item
    When the user presses ArrowDown
    Then the second ComboboxItem receives focus

  Scenario: ArrowUp moves focus to the previous item
    Given an open Combobox with focus on the second item
    When the user presses ArrowUp
    Then the first ComboboxItem receives focus

  Scenario: Enter selects the focused item
    Given an open Combobox with focus on an item
    When the user presses Enter
    Then that item's value is selected
    And the combobox closes

  Scenario: Escape closes the combobox
    Given an open Combobox
    When the user presses Escape
    Then the combobox closes

  Scenario: Tab closes the combobox
    Given an open Combobox
    When the user presses Tab
    Then the combobox closes
```

### 6. IsOpen is not externally bindable

`Combobox.IsOpen` is declared `internal`, so consumers cannot bind or control the open state from outside the component.

```gherkin
Feature: Combobox Controlled Open State
  As a developer
  I want to bind to the open state of the combobox
  So that I can control it programmatically

  Scenario: IsOpen can be set from outside
    Given a Combobox with IsOpen=true bound externally
    Then the dropdown is shown

  Scenario: IsOpenChanged fires when state changes
    Given a Combobox with an IsOpenChanged callback
    When the user opens the combobox
    Then IsOpenChanged is invoked with true
    When the user closes the combobox
    Then IsOpenChanged is invoked with false
```
---

## Navigation

[[components/_index|Back to Index]]