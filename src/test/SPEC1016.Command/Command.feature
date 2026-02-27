Feature: Command
  SPEC1016 | Component: Command

  Background:
    Given the component library LowlandTech.Components.Command

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Command renders data-slot attribute
    When a Command component is rendered
    Then it should have data-slot="command"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Command renders ChildContent
    When a Command component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Command renders Class parameter
    When a Command component is rendered with Class="test-class"
    Then the root element should have class "test-class"
