Feature: Tooltip
  SPEC1054 | Component: Tooltip

  Background:
    Given the component library LowlandTech.Components.Tooltip

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Tooltip renders data-slot attribute
    When a Tooltip component is rendered
    Then it should have data-slot="tooltip-provider"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Tooltip renders ChildContent
    When a Tooltip component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Tooltip renders Class parameter
    When a Tooltip component is rendered with Class="test-class"
    Then the root element should have class "test-class"
