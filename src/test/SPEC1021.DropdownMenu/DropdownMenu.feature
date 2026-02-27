Feature: DropdownMenu
  SPEC1021 | Component: DropdownMenu

  Background:
    Given the component library LowlandTech.Components.DropdownMenu

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 DropdownMenu renders data-slot attribute
    When a DropdownMenu component is rendered
    Then it should have data-slot="dropdown-menu"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 DropdownMenu renders ChildContent
    When a DropdownMenu component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 DropdownMenu renders Class parameter
    When a DropdownMenu component is rendered with Class="test-class"
    Then the root element should have class "test-class"
