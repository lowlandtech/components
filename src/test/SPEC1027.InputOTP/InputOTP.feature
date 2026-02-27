Feature: InputOTP
  SPEC1027 | Component: InputOTP

  Background:
    Given the component library LowlandTech.Components.InputOTP

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 InputOTP renders data-slot attribute
    When a InputOTP component is rendered
    Then it should have data-slot="input-otp"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 InputOTP renders ChildContent
    When a InputOTP component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 InputOTP renders Class parameter
    When a InputOTP component is rendered with Class="test-class"
    Then the root element should have class "test-class"
