Feature: Carousel
  SPEC1011 | Component: Carousel

  Background:
    Given the component library LowlandTech.Components.Carousel

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Carousel renders data-slot attribute
    When a Carousel component is rendered
    Then it should have data-slot="carousel"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Carousel renders ChildContent
    When a Carousel component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Carousel renders Class parameter
    When a Carousel component is rendered with Class="test-class"
    Then the root element should have class "test-class"
