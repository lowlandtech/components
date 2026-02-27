Feature: ContextMenu
  SPEC1017 | Component: ContextMenu

  Background:
    Given the component library LowlandTech.Components.ContextMenu

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 ContextMenu renders data-slot attribute
    When a ContextMenu component is rendered
    Then it should have data-slot="context-menu"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 ContextMenu renders ChildContent
    When a ContextMenu component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 ContextMenu renders Class parameter
    When a ContextMenu component is rendered with Class="test-class"
    Then the root element should have class "test-class"
