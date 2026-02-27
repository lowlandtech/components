Feature: Empty
  SPEC1022 | Component: Empty

  Background:
    Given the component library LowlandTech.Components.Empty

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Empty renders data-slot attribute
    When a Empty component is rendered
    Then it should have data-slot="empty"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Empty renders ChildContent
    When a Empty component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Empty renders Class parameter
    When a Empty component is rendered with Class="test-class"
    Then the root element should have class "test-class"
