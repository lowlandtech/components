Feature: Popover
  SPEC1035 | Component: Popover

  Background:
    Given the component library LowlandTech.Components.Popover

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Popover renders data-slot attribute
    When a Popover component is rendered
    Then it should have data-slot="popover"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Popover renders ChildContent
    When a Popover component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Popover renders Class parameter
    When a Popover component is rendered with Class="test-class"
    Then the root element should have class "test-class"
