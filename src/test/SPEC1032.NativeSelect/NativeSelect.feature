Feature: NativeSelect
  SPEC1032 | Component: NativeSelect

  Background:
    Given the component library LowlandTech.Components.NativeSelect

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 NativeSelect renders data-slot attribute
    When a NativeSelect component is rendered
    Then it should have data-slot="native-select"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 NativeSelect renders ChildContent
    When a NativeSelect component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 NativeSelect renders Class parameter
    When a NativeSelect component is rendered with Class="test-class"
    Then the root element should have class "test-class"
