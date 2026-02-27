Feature: Menubar
  SPEC1031 | Component: Menubar

  Background:
    Given the component library LowlandTech.Components.Menubar

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Menubar renders data-slot attribute
    When a Menubar component is rendered
    Then it should have data-slot="menubar"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Menubar renders ChildContent
    When a Menubar component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Menubar renders Class parameter
    When a Menubar component is rendered with Class="test-class"
    Then the root element should have class "test-class"
