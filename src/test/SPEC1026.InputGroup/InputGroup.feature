Feature: InputGroup
  SPEC1026 | Component: InputGroup

  Background:
    Given the component library LowlandTech.Components.InputGroup

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 InputGroup renders data-slot attribute
    When a InputGroup component is rendered
    Then it should have data-slot="input-group"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 InputGroup renders ChildContent
    When a InputGroup component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 InputGroup renders Class parameter
    When a InputGroup component is rendered with Class="test-class"
    Then the root element should have class "test-class"
