Feature: Calendar
  SPEC1009 | Component: Calendar

  Background:
    Given the component library LowlandTech.Components.Calendar

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Calendar renders data-slot attribute
    When a Calendar component is rendered
    Then it should have data-slot="calendar"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Calendar renders ChildContent
    When a Calendar component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Calendar renders Class parameter
    When a Calendar component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US02 @modes @SC01
  Scenario: US02-SC01-UAC01 Calendar Mode single selects one date
    When a Calendar is rendered in Single mode
    And day 15 is clicked
    Then SelectedDate should be day 15

  @US02 @modes @SC01
  Scenario: US02-SC01-UAC02 Single mode replaces previous selection
    When a Calendar is rendered with SelectedDate set to day 10
    And day 20 is clicked
    Then SelectedDate should be day 20

  @US02 @modes @SC02
  Scenario: US02-SC02-UAC01 Calendar Mode range selects start and end
    When a Calendar is rendered in Range mode
    And day 15 is clicked
    And day 20 is clicked
    Then SelectedRange Start should be day 15
    And SelectedRange End should be day 20

  @US02 @modes @SC03
  Scenario: US02-SC03-UAC01 Calendar Mode multiple selects multiple dates
    When a Calendar is rendered in Multiple mode
    And day 10 is clicked
    And day 15 is clicked
    And day 20 is clicked
    Then SelectedDates should contain days 10, 15, and 20

  @US03 @navigation @SC01
  Scenario: US03-SC01-UAC01 Calendar navigation previous month works
    When a Calendar is rendered showing June 2024
    And the previous month button is clicked
    Then the calendar should show May 2024

  @US03 @navigation @SC01
  Scenario: US03-SC01-UAC02 Calendar navigation next month works
    When a Calendar is rendered showing June 2024
    And the next month button is clicked
    Then the calendar should show July 2024

  @US03 @navigation @SC02
  Scenario: US03-SC02-UAC01 Calendar navigation previous year works
    When a Calendar is rendered showing June 2024
    And the previous year button is clicked
    Then the calendar should show June 2023

  @US03 @navigation @SC02
  Scenario: US03-SC02-UAC02 Calendar navigation next year works
    When a Calendar is rendered showing June 2024
    And the next year button is clicked
    Then the calendar should show June 2025

  @US04 @attributes @SC01
  Scenario: US04-SC01-UAC01 Calendar day data-selected reflects selection
    When a Calendar is rendered in Single mode
    And day 15 is clicked
    Then day 15 cell should have data-selected="True"

  @US04 @attributes @SC01
  Scenario: US04-SC01-UAC03 Calendar day data-today marks today
    When a Calendar is rendered for the current month
    Then today's cell should have data-today="True"

  @US04 @attributes @SC01
  Scenario: US04-SC01-UAC05 Calendar day data-outside marks outside month days
    When a Calendar is rendered showing June 2024
    Then days outside June should have data-outside="True"

  @US04 @attributes @SC02
  Scenario: US04-SC02-UAC01 Calendar day data-disabled reflects disabled
    When a Calendar is rendered with IsDateDisabled disabling day 15
    Then day 15 cell should have data-disabled="True"
