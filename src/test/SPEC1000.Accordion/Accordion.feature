Feature: Accordion
  SPEC1000 | Component: Accordion
  Implements a collapsible content panel system with single/multiple expansion modes.
  Sub-components: AccordionItem, AccordionTrigger, AccordionContent

  Background:
    Given the component library LowlandTech.Components.Accordion

  # --- US01: View — Accordion Root ---

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Accordion renders data-slot attribute
    When an Accordion component is rendered
    Then it should have data-slot="accordion"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Accordion renders ChildContent
    When an Accordion component is rendered with child content "Hello"
    Then the markup should contain "Hello"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Accordion renders Class parameter
    When an Accordion component is rendered with Class="test-class"
    Then the root element should have class "test-class"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Accordion renders default accordion CSS class
    When an Accordion component is rendered
    Then the root element should have class "accordion"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Accordion renders data-type attribute
    When an Accordion component is rendered with Type="multiple"
    Then the root element should have data-type="multiple"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC06 Accordion defaults to single type
    When an Accordion component is rendered with defaults
    Then data-type should be "single"

  # --- US01: View — AccordionItem ---

  @US01 @view @SC02
  Scenario: US01-SC02-UAC01 AccordionItem renders data-slot attribute
    When an AccordionItem is rendered inside Accordion
    Then it should have data-slot="accordion-item"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC02 AccordionItem renders default CSS class
    When an AccordionItem is rendered
    Then the element should have class "accordion-item"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC03 AccordionItem renders data-state closed by default
    When an AccordionItem is rendered without DefaultValue
    Then data-state should be "closed"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC04 AccordionItem renders data-state open when DefaultValue matches
    When Accordion has DefaultValue="item-1" and item Value="item-1"
    Then data-state should be "open"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC05 AccordionItem renders data-disabled when disabled
    When an AccordionItem has Disabled=true
    Then data-disabled should be "true"

  # --- US01: View — AccordionTrigger ---

  @US01 @view @SC03
  Scenario: US01-SC03-UAC01 AccordionTrigger renders data-slot attribute
    When an AccordionTrigger is rendered inside AccordionItem
    Then it should have data-slot="accordion-trigger"

  @US01 @view @SC03
  Scenario: US01-SC03-UAC02 AccordionTrigger renders as button element
    When an AccordionTrigger is rendered
    Then the element tag should be "BUTTON"

  @US01 @view @SC03
  Scenario: US01-SC03-UAC03 AccordionTrigger renders default CSS class
    When an AccordionTrigger is rendered
    Then the element should have class "accordion-trigger"

  @US01 @view @SC03
  Scenario: US01-SC03-UAC04 AccordionTrigger renders ChildContent
    When an AccordionTrigger is rendered with child "My Trigger"
    Then the trigger text should contain "My Trigger"

  @US01 @view @SC03
  Scenario: US01-SC03-UAC05 AccordionTrigger data-state is closed by default
    When an AccordionTrigger is rendered without open item
    Then data-state should be "closed"

  # --- US01: View — AccordionContent ---

  @US01 @view @SC04
  Scenario: US01-SC04-UAC01 AccordionContent is hidden when item is closed
    When an AccordionContent is rendered inside a closed item
    Then the content should not be in the DOM

  @US01 @view @SC04
  Scenario: US01-SC04-UAC02 AccordionContent is visible when item is open
    When an AccordionContent is rendered inside an open item (via DefaultValue)
    Then the content should be visible in the DOM

  @US01 @view @SC04
  Scenario: US01-SC04-UAC03 AccordionContent renders default CSS class when open
    When an AccordionContent is visible
    Then the element should have class "accordion-content"

  @US01 @view @SC04
  Scenario: US01-SC04-UAC04 AccordionContent renders data-state open
    When an AccordionContent is visible
    Then data-state should be "open"

  # --- US01: View — Accordion Behavior ---

  @US01 @view @SC05
  Scenario: US01-SC05-UAC01 Clicking trigger opens accordion content
    Given a collapsible single-mode Accordion with one item
    When the trigger is clicked
    Then content becomes visible

  @US01 @view @SC05
  Scenario: US01-SC05-UAC02 Clicking trigger again closes content in collapsible mode
    Given a collapsible single-mode Accordion with an open item
    When the trigger is clicked again
    Then content is hidden again

  @US01 @view @SC05
  Scenario: US01-SC05-UAC03 Single mode — opening second item closes first
    Given a single-mode Accordion with two items
    When the first item is opened then the second is opened
    Then only the second item is open

  @US01 @view @SC05
  Scenario: US01-SC05-UAC04 Multiple mode — both items can be open
    Given a multiple-mode Accordion with two items
    When both triggers are clicked
    Then both items have visible content

  @US01 @view @SC05
  Scenario: US01-SC05-UAC05 Disabled item trigger does not open content
    Given an Accordion with a disabled item
    When the disabled trigger is clicked
    Then content stays hidden

  @US01 @view @SC05
  Scenario: US01-SC05-UAC06 DefaultValue opens item on initial render
    Given an Accordion with DefaultValue="item-1"
    When the component renders
    Then item is open on first render

  # --- US02: State — AccordionState ---

  @US02 @state @SC01
  Scenario: US02-SC01-UAC01 Initial state has no open items
    Given a new AccordionState
    Then no items are open

  @US02 @state @SC01
  Scenario: US02-SC01-UAC02 Toggle opens an item
    Given a new AccordionState
    When Toggle("item-1") is called
    Then the item is open

  @US02 @state @SC01
  Scenario: US02-SC01-UAC03 Single mode closes current when opening new
    Given AccordionState with Type="single" and Collapsible=true
    When Toggle("item-1") then Toggle("item-2")
    Then only item-2 is open

  @US02 @state @SC01
  Scenario: US02-SC01-UAC04 Single mode non-collapsible cannot close open item
    Given AccordionState with Type="single" and Collapsible=false
    When Toggle("item-1") twice
    Then the item stays open

  @US02 @state @SC01
  Scenario: US02-SC01-UAC05 Single mode collapsible can close open item
    Given AccordionState with Type="single" and Collapsible=true
    When Toggle("item-1") twice
    Then the item closes

  @US02 @state @SC01
  Scenario: US02-SC01-UAC06 Multiple mode allows multiple open items
    Given AccordionState with Type="multiple"
    When Toggle("item-1") and Toggle("item-2")
    Then both items are open

  @US02 @state @SC01
  Scenario: US02-SC01-UAC07 Multiple mode toggle closes individual items
    Given AccordionState with Type="multiple" and two open items
    When Toggle("item-1") is called again
    Then only item-2 remains open

  @US02 @state @SC01
  Scenario: US02-SC01-UAC08 OnChange fires on Toggle
    Given AccordionState with an OnChange subscriber
    When Toggle is called
    Then the change event is raised

  @US02 @state @SC01
  Scenario: US02-SC01-UAC09 SetOpen opens item in single mode
    Given AccordionState with Type="single"
    When SetOpen("item-1") is called
    Then the item is open
