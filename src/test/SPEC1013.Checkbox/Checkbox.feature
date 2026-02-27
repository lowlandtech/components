Feature: Checkbox
  SPEC1013 | Component: Checkbox

  Background:
    Given the component library LowlandTech.Components.Checkbox

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Checkbox renders data-slot attribute
    When a Checkbox component is rendered
    Then it should have data-slot="checkbox"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Checkbox renders ChildContent
    When a Checkbox component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Checkbox renders Class parameter
    When a Checkbox component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Checkbox renders as button element
    When a Checkbox component is rendered
    Then the root element should be a "button"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Checkbox renders @checked parameter
    When a Checkbox component is rendered with @checked=true
    Then the component should expose @checked boolean parameter
