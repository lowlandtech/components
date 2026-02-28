@US01
Feature: US01 - View Alert Component
  SPEC1001 | Render Alert and sub-components with variant styling, slots, and composition

  Background:
    Given the component library LowlandTech.Components.Alert

  # ── Alert (root) ──────────────────────────────────────────────

  @SC01
  Scenario: Render Alert root with defaults
    When an Alert component is rendered with no parameters
    #UAC01
    Then it should have data-slot="alert"
    #UAC02
    And the root element should be a "div"
    #UAC03
    And the root element should have role="alert"
    #UAC04
    And the root element should have class "alert"
    #UAC05
    And the root element should have data-variant="default"

  @SC02
  Scenario: Render Alert with parameters
    When an Alert component is rendered with Variant="destructive" and Class="test-class"
    #UAC06
    Then the root element should have data-variant="destructive"
    #UAC07
    And the root element should have class "test-class"
    #UAC08
    And the root element should have class "alert"

  @SC03
  Scenario: Render Alert with child content
    When an Alert component is rendered with child content "Hello"
    #UAC09
    Then the markup should contain "Hello"

  @SC04
  Scenario: Render Alert with additional attributes
    When an Alert component is rendered with aria-label="notification"
    #UAC10
    Then the root element should have aria-label="notification"

  # ── AlertTitle ────────────────────────────────────────────────

  @SC05
  Scenario: Render AlertTitle with defaults
    When an AlertTitle component is rendered
    #UAC11
    Then it should have data-slot="alert-title"
    #UAC12
    And the root element should have class "alert-title"

  @SC06
  Scenario: Render AlertTitle with content and class
    When an AlertTitle is rendered with child content "Heads up!" and Class="custom"
    #UAC13
    Then the markup should contain "Heads up!"
    #UAC14
    And the root element should have class "alert-title"
    #UAC15
    And the root element should have class "custom"

  @SC07
  Scenario: Render AlertTitle with anchor links
    When an AlertTitle is rendered with child content containing an anchor tag
    #UAC16
    Then the alert-title slot should contain an "a" element

  # ── AlertDescription ──────────────────────────────────────────

  @SC08
  Scenario: Render AlertDescription with defaults
    When an AlertDescription component is rendered
    #UAC17
    Then it should have data-slot="alert-description"
    #UAC18
    And the root element should have class "alert-description"

  @SC09
  Scenario: Render AlertDescription with content and class
    When an AlertDescription is rendered with child content "You can add components." and Class="custom"
    #UAC19
    Then the markup should contain "You can add components."
    #UAC20
    And the root element should have class "alert-description"
    #UAC21
    And the root element should have class "custom"

  @SC10
  Scenario: Render AlertDescription with rich HTML content
    When an AlertDescription is rendered with a paragraph, an unordered list, and anchor tags
    #UAC22
    Then the alert-description slot should contain a "p" element
    #UAC23
    And the alert-description slot should contain a "ul" element
    #UAC24
    And the alert-description slot should contain "a" elements

  # ── AlertAction ───────────────────────────────────────────────

  @SC11
  Scenario: Render AlertAction with defaults
    When an AlertAction component is rendered
    #UAC25
    Then it should have data-slot="alert-action"
    #UAC26
    And the root element should have class "alert-action"

  @SC12
  Scenario: Render AlertAction with content and class
    When an AlertAction is rendered with child content "<button>Enable</button>" and Class="custom"
    #UAC27
    Then the markup should contain "Enable"
    #UAC28
    And the root element should have class "alert-action"
    #UAC29
    And the root element should have class "custom"

  @SC13
  Scenario: Render AlertAction with interactive element
    When an AlertAction is rendered containing a button "Undo"
    #UAC30
    Then the alert-action slot should contain a button element
    #UAC31
    And the button should contain "Undo"

  # ── Composition ───────────────────────────────────────────────

  @SC14
  Scenario: Compose Alert with title, description, and action
    When an Alert is rendered containing AlertTitle, AlertDescription, and AlertAction
    #UAC32
    Then all sub-component data-slots should be present inside the alert
    #UAC33
    And the content order should be title, description, action

  @SC15
  Scenario: Compose destructive Alert with all sub-components
    When a destructive Alert is rendered with title "Error" and description "Something failed"
    #UAC34
    Then the alert should have data-variant="destructive"
    #UAC35
    And the alert-title slot should contain "Error"
    #UAC36
    And the alert-description slot should contain "Something failed"

  @SC16
  Scenario: Compose Alert with title only
    When an Alert is rendered containing only an AlertTitle "Success! Your changes have been saved."
    #UAC37
    Then the alert-title slot should contain "Success! Your changes have been saved."
    #UAC38
    And no alert-description slot should be present

  @SC17
  Scenario: Compose Alert with description only
    When an Alert is rendered containing only an AlertDescription "This one has a description only."
    #UAC39
    Then the alert-description slot should contain "This one has a description only."
    #UAC40
    And no alert-title slot should be present

  @SC18
  Scenario: Compose Alert with icon child
    When an Alert is rendered with an SVG icon as a direct child
    #UAC41
    Then the alert should contain an svg element

  @SC19
  Scenario: Compose Alert with icon, title, and description
    When an Alert is rendered with an SVG icon, AlertTitle, and AlertDescription
    #UAC42
    Then the alert should contain an svg element
    #UAC43
    And the alert-title slot should be present
    #UAC44
    And the alert-description slot should be present

  @SC20
  Scenario: Compose Alert with icon, title, and action
    When an Alert is rendered with an SVG icon, AlertTitle, and AlertAction
    #UAC45
    Then the alert should contain an svg element
    #UAC46
    And the alert-title slot should be present
    #UAC47
    And the alert-action slot should be present
    #UAC48
    And no alert-description slot should be present
