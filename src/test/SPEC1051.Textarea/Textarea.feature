Feature: Textarea
  SPEC1051 | Component: Textarea

  Background:
    Given the component library LowlandTech.Components.Textarea

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Textarea renders data-slot attribute
    When a Textarea component is rendered
    Then it should have data-slot="textarea"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Textarea renders ChildContent
    When a Textarea component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Textarea renders Class parameter
    When a Textarea component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Textarea renders as textarea element
    When a Textarea component is rendered
    Then the root element should be a "textarea"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Textarea renders placeholder parameter
    When a Textarea component is rendered with placeholder="Enter text"
    Then the component should expose placeholder parameter

  @US01 @view @SC01
  Scenario: US01-SC01-UAC06 Textarea renders rows parameter
    When a Textarea component is rendered with rows="5"
    Then the component should expose rows parameter
