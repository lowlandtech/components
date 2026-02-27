Feature: Chart
  SPEC1012 | Component: Chart

  Background:
    Given the component library LowlandTech.Components.Chart

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Chart renders data-slot attribute
    When a Chart component is rendered
    Then it should have data-slot="chart"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Chart renders ChildContent
    When a Chart component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Chart renders Class parameter
    When a Chart component is rendered with Class="test-class"
    Then the root element should have class "test-class"
