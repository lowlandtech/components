Feature: Button
  SPEC1007 | Component: Button

  Background:
    Given the component library LowlandTech.Components.Button

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Button renders data-slot attribute
    When a Button component is rendered
    Then it should have data-slot="button"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Button renders ChildContent
    When a Button component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Button renders Class parameter
    When a Button component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Button renders as button element
    When a Button component is rendered
    Then the root element should be a "button"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Button renders variant parameter
    When a Button component is rendered with variant="outline"
    Then the component should expose variant parameter

  @US01 @view @SC01
  Scenario: US01-SC01-UAC06 Button renders size parameter
    When a Button component is rendered with size="sm"
    Then the component should expose size parameter
