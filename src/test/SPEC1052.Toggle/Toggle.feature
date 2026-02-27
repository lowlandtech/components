Feature: Toggle
  SPEC1052 | Component: Toggle

  Background:
    Given the component library LowlandTech.Components.Toggle

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Toggle renders data-slot attribute
    When a Toggle component is rendered
    Then it should have data-slot="toggle"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Toggle renders ChildContent
    When a Toggle component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Toggle renders Class parameter
    When a Toggle component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Toggle renders as button element
    When a Toggle component is rendered
    Then the root element should be a "button"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Toggle renders pressed parameter
    When a Toggle component is rendered with pressed=true
    Then the component should expose pressed boolean parameter
