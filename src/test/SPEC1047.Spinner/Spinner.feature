Feature: Spinner
  SPEC1047 | Component: Spinner

  Background:
    Given the component library LowlandTech.Components.Spinner

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Spinner renders data-slot attribute
    When a Spinner component is rendered
    Then it should have data-slot="spinner"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Spinner renders ChildContent
    When a Spinner component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Spinner renders Class parameter
    When a Spinner component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Spinner renders as span element
    When a Spinner component is rendered
    Then the root element should be a "span"
