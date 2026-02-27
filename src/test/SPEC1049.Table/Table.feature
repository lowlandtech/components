Feature: Table
  SPEC1049 | Component: Table

  Background:
    Given the component library LowlandTech.Components.Table

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Table renders data-slot attribute
    When a Table component is rendered
    Then it should have data-slot="table"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Table renders ChildContent
    When a Table component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Table renders Class parameter
    When a Table component is rendered with Class="test-class"
    Then the root element should have class "test-class"
