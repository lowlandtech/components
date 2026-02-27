Feature: ToggleGroup
  SPEC1053 | Component: ToggleGroup

  Background:
    Given the component library LowlandTech.Components.ToggleGroup

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 ToggleGroup renders data-slot attribute
    When a ToggleGroup component is rendered
    Then it should have data-slot="toggle-group"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 ToggleGroup renders ChildContent
    When a ToggleGroup component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 ToggleGroup renders Class parameter
    When a ToggleGroup component is rendered with Class="test-class"
    Then the root element should have class "test-class"
