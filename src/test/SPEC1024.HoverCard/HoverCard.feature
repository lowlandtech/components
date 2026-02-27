Feature: HoverCard
  SPEC1024 | Component: HoverCard

  Background:
    Given the component library LowlandTech.Components.HoverCard

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 HoverCard renders data-slot attribute
    When a HoverCard component is rendered
    Then it should have data-slot="hover-card"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 HoverCard renders ChildContent
    When a HoverCard component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 HoverCard renders Class parameter
    When a HoverCard component is rendered with Class="test-class"
    Then the root element should have class "test-class"
