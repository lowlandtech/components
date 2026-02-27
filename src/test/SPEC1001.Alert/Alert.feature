Feature: Alert
  SPEC1001 | Component: Alert

  Background:
    Given the component library LowlandTech.Components.Alert

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Alert renders data-slot attribute
    When a Alert component is rendered
    Then it should have data-slot="alert"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Alert renders ChildContent
    When a Alert component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Alert renders Class parameter
    When a Alert component is rendered with Class="test-class"
    Then the root element should have class "test-class"
