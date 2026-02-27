Feature: Drawer
  SPEC1020 | Component: Drawer

  Background:
    Given the component library LowlandTech.Components.Drawer

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Drawer renders data-slot attribute
    When a Drawer component is rendered
    Then it should have data-slot="drawer"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Drawer renders ChildContent
    When a Drawer component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Drawer renders Class parameter
    When a Drawer component is rendered with Class="test-class"
    Then the root element should have class "test-class"
