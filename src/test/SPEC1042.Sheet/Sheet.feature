Feature: Sheet
  SPEC1042 | Component: Sheet

  Background:
    Given the component library LowlandTech.Components.Sheet

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Sheet renders data-slot attribute
    When a Sheet component is rendered
    Then it should have data-slot="sheet"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Sheet renders ChildContent
    When a Sheet component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Sheet renders Class parameter
    When a Sheet component is rendered with Class="test-class"
    Then the root element should have class "test-class"
