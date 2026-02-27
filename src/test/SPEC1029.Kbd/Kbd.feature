Feature: Kbd
  SPEC1029 | Component: Kbd

  Background:
    Given the component library LowlandTech.Components.Kbd

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Kbd renders data-slot attribute
    When a Kbd component is rendered
    Then it should have data-slot="kbd"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Kbd renders ChildContent
    When a Kbd component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Kbd renders Class parameter
    When a Kbd component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Kbd renders as kbd element
    When a Kbd component is rendered
    Then the root element should be a "kbd"
