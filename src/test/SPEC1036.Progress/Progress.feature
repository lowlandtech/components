Feature: Progress
  SPEC1036 | Component: Progress

  Background:
    Given the component library LowlandTech.Components.Progress

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Progress renders data-slot attribute
    When a Progress component is rendered
    Then it should have data-slot="progress"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Progress renders ChildContent
    When a Progress component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Progress renders Class parameter
    When a Progress component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Progress renders value parameter
    When a Progress component is rendered with value="50"
    Then the component should expose value parameter

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Progress renders max parameter
    When a Progress component is rendered with max="100"
    Then the component should expose max parameter
