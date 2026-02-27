Feature: Separator
  SPEC1041 | Component: Separator

  Background:
    Given the component library LowlandTech.Components.Separator

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Separator renders data-slot attribute
    When a Separator component is rendered
    Then it should have data-slot="separator"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Separator renders ChildContent
    When a Separator component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Separator renders Class parameter
    When a Separator component is rendered with Class="test-class"
    Then the root element should have class "test-class"
