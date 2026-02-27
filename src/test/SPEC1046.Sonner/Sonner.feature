Feature: Sonner
  SPEC1046 | Component: Sonner

  Background:
    Given the component library LowlandTech.Components.Sonner

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Sonner renders data-slot attribute
    When a Sonner component is rendered
    Then it should have data-slot="sonner-toaster"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Sonner renders ChildContent
    When a Sonner component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Sonner renders Class parameter
    When a Sonner component is rendered with Class="test-class"
    Then the root element should have class "test-class"
