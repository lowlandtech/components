Feature: Select
  SPEC1040 | Component: Select

  Background:
    Given the component library LowlandTech.Components.Select

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Select renders data-slot attribute
    When a Select component is rendered
    Then it should have data-slot="select"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Select renders ChildContent
    When a Select component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Select renders Class parameter
    When a Select component is rendered with Class="test-class"
    Then the root element should have class "test-class"
