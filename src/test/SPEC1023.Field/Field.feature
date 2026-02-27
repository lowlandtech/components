Feature: Field
  SPEC1023 | Component: Field

  Background:
    Given the component library LowlandTech.Components.Field

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Field renders data-slot attribute
    When a Field component is rendered
    Then it should have data-slot="field"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Field renders ChildContent
    When a Field component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Field renders Class parameter
    When a Field component is rendered with Class="test-class"
    Then the root element should have class "test-class"
