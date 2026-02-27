Feature: Dialog
  SPEC1018 | Component: Dialog

  Background:
    Given the component library LowlandTech.Components.Dialog

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Dialog renders data-slot attribute
    When a Dialog component is rendered
    Then it should have data-slot="dialog"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Dialog renders ChildContent
    When a Dialog component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Dialog renders Class parameter
    When a Dialog component is rendered with Class="test-class"
    Then the root element should have class "test-class"
