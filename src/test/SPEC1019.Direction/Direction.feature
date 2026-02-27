Feature: Direction
  SPEC1019 | Component: Direction

  Background:
    Given the component library LowlandTech.Components.Direction

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Direction renders data-slot attribute
    When a Direction component is rendered
    Then it should have data-slot="direction-provider"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Direction renders ChildContent
    When a Direction component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Direction renders Class parameter
    When a Direction component is rendered with Class="test-class"
    Then the root element should have class "test-class"
