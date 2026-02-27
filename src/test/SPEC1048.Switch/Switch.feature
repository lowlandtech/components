Feature: Switch
  SPEC1048 | Component: Switch

  Background:
    Given the component library LowlandTech.Components.Switch

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Switch renders data-slot attribute
    When a Switch component is rendered
    Then it should have data-slot="switch"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Switch renders ChildContent
    When a Switch component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Switch renders Class parameter
    When a Switch component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Switch renders as button element
    When a Switch component is rendered
    Then the root element should be a "button"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Switch renders @checked parameter
    When a Switch component is rendered with @checked=true
    Then the component should expose @checked boolean parameter
