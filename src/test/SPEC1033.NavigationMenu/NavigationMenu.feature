Feature: NavigationMenu
  SPEC1033 | Component: NavigationMenu

  Background:
    Given the component library LowlandTech.Components.NavigationMenu

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 NavigationMenu renders data-slot attribute
    When a NavigationMenu component is rendered
    Then it should have data-slot="navigation-menu"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 NavigationMenu renders ChildContent
    When a NavigationMenu component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 NavigationMenu renders Class parameter
    When a NavigationMenu component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 NavigationMenu renders as nav element
    When a NavigationMenu component is rendered
    Then the root element should be a "nav"
