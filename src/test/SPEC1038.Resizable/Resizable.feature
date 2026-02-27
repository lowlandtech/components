Feature: Resizable
  SPEC1038 | Component: Resizable

  Background:
    Given the component library LowlandTech.Components.Resizable

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Resizable renders data-slot attribute
    When a Resizable component is rendered
    Then it should have data-slot="resizable-panel-group"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Resizable renders ChildContent
    When a Resizable component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Resizable renders Class parameter
    When a Resizable component is rendered with Class="test-class"
    Then the root element should have class "test-class"
