Feature: Tabs
  SPEC1050 | Component: Tabs

  Background:
    Given the component library LowlandTech.Components.Tabs

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Tabs renders data-slot attribute
    When a Tabs component is rendered
    Then it should have data-slot="tabs"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Tabs renders ChildContent
    When a Tabs component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Tabs renders Class parameter
    When a Tabs component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC09 Tabs renders with default top placement
    When a Tabs component is rendered
    Then it should have data-placement="top"
    And it should use flex-col layout

  @US01 @view @SC01
  Scenario: US01-SC01-UAC10 Tabs renders with bottom placement
    When a Tabs component is rendered with Placement="bottom"
    Then it should have data-placement="bottom"
    And it should use flex-col-reverse layout

  @US01 @view @SC01
  Scenario: US01-SC01-UAC11 Tabs renders with left placement
    When a Tabs component is rendered with Placement="left"
    Then it should have data-placement="left"
    And it should use flex-row layout

  @US01 @view @SC01
  Scenario: US01-SC01-UAC12 Tabs renders with right placement
    When a Tabs component is rendered with Placement="right"
    Then it should have data-placement="right"
    And it should use flex-row-reverse layout

  @US01 @view @SC01
  Scenario: US01-SC01-UAC13 Left placement derives vertical orientation
    When a Tabs component is rendered with Placement="left"
    Then it should have data-orientation="vertical"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC14 Right placement derives vertical orientation
    When a Tabs component is rendered with Placement="right"
    Then it should have data-orientation="vertical"
