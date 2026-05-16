Feature: Calendar View Zoom
  As a user
  I want to zoom out from the day grid to pick months and years
  So that I can navigate dates faster without scrolling month by month

  Background:
    Given the Calendar component is rendered with AllowViewZoom enabled

  Scenario: Clicking caption zooms to month picker
    When I click the calendar caption in day view
    Then the calendar switches to month picker view
    And 12 month buttons are displayed

  Scenario: Selecting a month returns to day view
    Given the calendar is in month picker view
    When I click a month button
    Then the calendar switches back to day view
    And the selected month is displayed

  Scenario: Clicking caption in month view zooms to year picker
    Given the calendar is in month picker view
    When I click the calendar caption
    Then the calendar switches to year picker view
    And 12 year buttons are displayed

  Scenario: Selecting a year returns to month view
    Given the calendar is in year picker view
    When I click a year button
    Then the calendar switches to month picker view
    And the selected year is displayed

  Scenario: Year picker navigation
    Given the calendar is in year picker view
    When I click the previous navigation button
    Then the year range shifts back by 12 years
    When I click the next navigation button
    Then the year range shifts forward by 12 years
