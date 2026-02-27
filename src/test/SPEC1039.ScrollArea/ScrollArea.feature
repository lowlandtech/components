Feature: ScrollArea
  SPEC1039 | Component: ScrollArea

  Background:
    Given the component library LowlandTech.Components.ScrollArea

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 ScrollArea renders data-slot attribute
    When a ScrollArea component is rendered
    Then it should have data-slot="scroll-area"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 ScrollArea renders ChildContent
    When a ScrollArea component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 ScrollArea renders Class parameter
    When a ScrollArea component is rendered with Class="test-class"
    Then the root element should have class "test-class"
