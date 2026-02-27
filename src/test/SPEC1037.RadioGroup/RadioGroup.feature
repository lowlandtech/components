Feature: RadioGroup
  SPEC1037 | Component: RadioGroup

  Background:
    Given the component library LowlandTech.Components.RadioGroup

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 RadioGroup renders data-slot attribute
    When a RadioGroup component is rendered
    Then it should have data-slot="radio-group"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 RadioGroup renders ChildContent
    When a RadioGroup component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 RadioGroup renders Class parameter
    When a RadioGroup component is rendered with Class="test-class"
    Then the root element should have class "test-class"
