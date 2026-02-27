Feature: AspectRatio
  SPEC1003 | Component: AspectRatio

  Background:
    Given the component library LowlandTech.Components.AspectRatio

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 AspectRatio renders data-slot attribute
    When a AspectRatio component is rendered
    Then it should have data-slot="aspect-ratio"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 AspectRatio renders ChildContent
    When a AspectRatio component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 AspectRatio renders Class parameter
    When a AspectRatio component is rendered with Class="test-class"
    Then the root element should have class "test-class"
