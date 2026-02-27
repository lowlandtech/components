Feature: Calendar
  SPEC1009 | Component: Calendar

  Background:
    Given the component library LowlandTech.Components.Calendar

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Calendar renders data-slot attribute
    When a Calendar component is rendered
    Then it should have data-slot="calendar"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Calendar renders ChildContent
    When a Calendar component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Calendar renders Class parameter
    When a Calendar component is rendered with Class="test-class"
    Then the root element should have class "test-class"
