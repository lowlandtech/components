Feature: Slider
  SPEC1045 | Component: Slider

  Background:
    Given the component library LowlandTech.Components.Slider

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Slider renders data-slot attribute
    When a Slider component is rendered
    Then it should have data-slot="slider"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Slider renders ChildContent
    When a Slider component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Slider renders Class parameter
    When a Slider component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Slider renders as span element
    When a Slider component is rendered
    Then the root element should be a "span"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Slider renders min parameter
    When a Slider component is rendered with min="0"
    Then the component should expose min parameter

  @US01 @view @SC01
  Scenario: US01-SC01-UAC06 Slider renders max parameter
    When a Slider component is rendered with max="100"
    Then the component should expose max parameter

  @US01 @view @SC01
  Scenario: US01-SC01-UAC07 Slider renders step parameter
    When a Slider component is rendered with step="1"
    Then the component should expose step parameter
