Feature: ButtonGroup
  SPEC1008 | Component: ButtonGroup

  Background:
    Given the component library LowlandTech.Components.ButtonGroup

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 ButtonGroup renders data-slot attribute
    When a ButtonGroup component is rendered
    Then it should have data-slot="button-group"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 ButtonGroup renders ChildContent
    When a ButtonGroup component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 ButtonGroup renders Class parameter
    When a ButtonGroup component is rendered with Class="test-class"
    Then the root element should have class "test-class"
