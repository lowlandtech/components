Feature: Skeleton
  SPEC1044 | Component: Skeleton

  Background:
    Given the component library LowlandTech.Components.Skeleton

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Skeleton renders data-slot attribute
    When a Skeleton component is rendered
    Then it should have data-slot="skeleton"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Skeleton renders ChildContent
    When a Skeleton component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Skeleton renders Class parameter
    When a Skeleton component is rendered with Class="test-class"
    Then the root element should have class "test-class"
