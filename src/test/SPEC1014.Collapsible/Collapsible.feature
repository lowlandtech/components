Feature: Collapsible
  SPEC1014 | Component: Collapsible

  Background:
    Given the component library LowlandTech.Components.Collapsible

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Collapsible renders data-slot attribute
    When a Collapsible component is rendered
    Then it should have data-slot="collapsible"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Collapsible renders ChildContent
    When a Collapsible component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Collapsible renders Class parameter
    When a Collapsible component is rendered with Class="test-class"
    Then the root element should have class "test-class"
