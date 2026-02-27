Feature: Avatar
  SPEC1004 | Component: Avatar

  Background:
    Given the component library LowlandTech.Components.Avatar

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Avatar renders data-slot attribute
    When a Avatar component is rendered
    Then it should have data-slot="avatar"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Avatar renders ChildContent
    When a Avatar component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Avatar renders Class parameter
    When a Avatar component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Avatar renders as span element
    When a Avatar component is rendered
    Then the root element should be a "span"
