Feature: Input
  SPEC1025 | Component: Input

  Background:
    Given the component library LowlandTech.Components.Input

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Input renders data-slot attribute
    When a Input component is rendered
    Then it should have data-slot="input"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Input renders ChildContent
    When a Input component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Input renders Class parameter
    When a Input component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Input renders as input element
    When a Input component is rendered
    Then the root element should be a "input"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Input renders type parameter
    When a Input component is rendered with type="text"
    Then the component should expose type parameter

  @US01 @view @SC01
  Scenario: US01-SC01-UAC06 Input renders placeholder parameter
    When a Input component is rendered with placeholder="Enter text"
    Then the component should expose placeholder parameter
