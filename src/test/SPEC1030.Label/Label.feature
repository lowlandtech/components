Feature: Label
  SPEC1030 | Component: Label

  Background:
    Given the component library LowlandTech.Components.Label

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Label renders data-slot attribute
    When a Label component is rendered
    Then it should have data-slot="label"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Label renders ChildContent
    When a Label component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Label renders Class parameter
    When a Label component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Label renders as label element
    When a Label component is rendered
    Then the root element should be a "label"
