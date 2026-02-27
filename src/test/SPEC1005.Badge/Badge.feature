Feature: Badge
  SPEC1005 | Component: Badge

  Background:
    Given the component library LowlandTech.Components.Badge

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Badge renders data-slot attribute
    When a Badge component is rendered
    Then it should have data-slot="badge"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Badge renders ChildContent
    When a Badge component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Badge renders Class parameter
    When a Badge component is rendered with Class="test-class"
    Then the root element should have class "test-class"
