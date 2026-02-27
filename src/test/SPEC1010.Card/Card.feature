Feature: Card
  SPEC1010 | Composable container for structured content with header, body, and footer

  Background:
    Given the component library LowlandTech.Components.Card

  # ── Card (root) ──────────────────────────────────────────────

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Card renders data-slot attribute
    When a Card component is rendered
    Then it should have data-slot="card"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Card renders ChildContent
    When a Card component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Card renders Class parameter
    When a Card component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Card renders default card CSS class
    When a Card component is rendered
    Then the root element should have class "card"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Card renders data-size attribute
    When a Card component is rendered with Size="sm"
    Then the root element should have data-size="sm"

  # ── CardHeader ───────────────────────────────────────────────

  @US01 @view @SC02
  Scenario: US01-SC02-UAC01 CardHeader renders data-slot attribute
    When a CardHeader component is rendered
    Then it should have data-slot="card-header"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC02 CardHeader renders ChildContent
    When a CardHeader component is rendered with child content "Header"
    Then the markup should contain "Header"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC03 CardHeader renders default CSS class
    When a CardHeader component is rendered
    Then the root element should have class "card-header"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC04 CardHeader merges custom Class parameter
    When a CardHeader component is rendered with Class="custom"
    Then the root element should have class "card-header"
    And the root element should have class "custom"

  # ── CardTitle ────────────────────────────────────────────────

  @US01 @view @SC03
  Scenario: US01-SC03-UAC01 CardTitle renders data-slot attribute
    When a CardTitle component is rendered
    Then it should have data-slot="card-title"

  @US01 @view @SC03
  Scenario: US01-SC03-UAC02 CardTitle renders as h3 element
    When a CardTitle component is rendered
    Then the root element should be a "h3"

  @US01 @view @SC03
  Scenario: US01-SC03-UAC03 CardTitle renders default CSS class
    When a CardTitle component is rendered
    Then the root element should have class "card-title"

  # ── CardDescription ─────────────────────────────────────────

  @US01 @view @SC04
  Scenario: US01-SC04-UAC01 CardDescription renders data-slot attribute
    When a CardDescription component is rendered
    Then it should have data-slot="card-description"

  @US01 @view @SC04
  Scenario: US01-SC04-UAC02 CardDescription renders as p element
    When a CardDescription component is rendered
    Then the root element should be a "p"

  @US01 @view @SC04
  Scenario: US01-SC04-UAC03 CardDescription renders default CSS class
    When a CardDescription component is rendered
    Then the root element should have class "card-description"

  # ── CardContent ──────────────────────────────────────────────

  @US01 @view @SC05
  Scenario: US01-SC05-UAC01 CardContent renders data-slot attribute
    When a CardContent component is rendered
    Then it should have data-slot="card-content"

  @US01 @view @SC05
  Scenario: US01-SC05-UAC02 CardContent renders default CSS class
    When a CardContent component is rendered
    Then the root element should have class "card-content"

  # ── CardFooter ───────────────────────────────────────────────

  @US01 @view @SC06
  Scenario: US01-SC06-UAC01 CardFooter renders data-slot attribute
    When a CardFooter component is rendered
    Then it should have data-slot="card-footer"

  @US01 @view @SC06
  Scenario: US01-SC06-UAC02 CardFooter renders default CSS class
    When a CardFooter component is rendered
    Then the root element should have class "card-footer"

  # ── CardAction ───────────────────────────────────────────────

  @US01 @view @SC07
  Scenario: US01-SC07-UAC01 CardAction renders data-slot attribute
    When a CardAction component is rendered
    Then it should have data-slot="card-action"

  @US01 @view @SC07
  Scenario: US01-SC07-UAC02 CardAction renders default CSS class
    When a CardAction component is rendered
    Then the root element should have class "card-action"

  # ── Composition ──────────────────────────────────────────────

  @US01 @view @SC08 @composition
  Scenario: US01-SC08-UAC01 Card composes header, content, and footer
    When a Card is rendered containing CardHeader, CardContent, and CardFooter
    Then all sub-component data-slots should be present inside the card
    And the content order should be header, content, footer

  @US01 @view @SC08 @composition
  Scenario: US01-SC08-UAC02 CardAction renders inside CardHeader
    When a CardHeader is rendered containing CardTitle and CardAction
    Then the card-action slot should be a child of card-header
