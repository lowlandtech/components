Feature: Item
  SPEC1028 | Component: Item

  Background:
    Given the component library LowlandTech.Components.Item

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Item renders data-slot attribute
    When a Item component is rendered
    Then it should have data-slot="item"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Item renders ChildContent
    When a Item component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Item renders Class parameter
    When a Item component is rendered with Class="test-class"
    Then the root element should have class "test-class"
