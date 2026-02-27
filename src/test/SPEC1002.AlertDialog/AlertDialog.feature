Feature: AlertDialog
  SPEC1002 | Component: AlertDialog

  Background:
    Given the component library LowlandTech.Components.AlertDialog

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 AlertDialog renders data-slot attribute
    When a AlertDialog component is rendered
    Then it should have data-slot="alert-dialog"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 AlertDialog renders ChildContent
    When a AlertDialog component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 AlertDialog renders Class parameter
    When a AlertDialog component is rendered with Class="test-class"
    Then the root element should have class "test-class"
