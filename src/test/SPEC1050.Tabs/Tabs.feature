Feature: Tabs
  SPEC1050 | Component: Tabs

  Background:
    Given the component library LowlandTech.Components.Tabs

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Tabs renders data-slot attribute
    When a Tabs component is rendered
    Then it should have data-slot="tabs"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Tabs renders ChildContent
    When a Tabs component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Tabs renders Class parameter
    When a Tabs component is rendered with Class="test-class"
    Then the root element should have class "test-class"
