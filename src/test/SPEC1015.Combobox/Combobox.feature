Feature: Combobox
  SPEC1015 | Component: Combobox

  Background:
    Given the component library LowlandTech.Components.Combobox

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Combobox renders data-slot attribute
    When a Combobox component is rendered
    Then it should have data-slot="combobox"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Combobox renders ChildContent
    When a Combobox component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Combobox renders Class parameter
    When a Combobox component is rendered with Class="test-class"
    Then the root element should have class "test-class"
