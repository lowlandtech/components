Feature: Breadcrumb
  SPEC1006 | Component: Breadcrumb

  Background:
    Given the component library LowlandTech.Components.Breadcrumb

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Breadcrumb renders data-slot attribute
    When a Breadcrumb component is rendered
    Then it should have data-slot="breadcrumb"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Breadcrumb renders ChildContent
    When a Breadcrumb component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Breadcrumb renders Class parameter
    When a Breadcrumb component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Breadcrumb renders as nav element
    When a Breadcrumb component is rendered
    Then the root element should be a "nav"
