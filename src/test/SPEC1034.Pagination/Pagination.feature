Feature: Pagination
  SPEC1034 | Component: Pagination

  Background:
    Given the component library LowlandTech.Components.Pagination

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Pagination renders data-slot attribute
    When a Pagination component is rendered
    Then it should have data-slot="pagination"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Pagination renders ChildContent
    When a Pagination component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Pagination renders Class parameter
    When a Pagination component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Pagination renders as nav element
    When a Pagination component is rendered
    Then the root element should be a "nav"
