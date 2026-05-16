@US:06
Feature: SPEC1072.US06 - Rendering Extensibility and Accessibility
  As a design-system user
  I want DataForm to expose templates, slots, layout options, and accessible validation markup
  So that generated forms can be customized without losing validation behavior.

  Background:
    Given the component library LowlandTech.Components.DataForm
    And DataForm renders fields discovered from a model

  # STATE
  # interface DataFormRenderingState {
  #   fields: DataFormFieldRepresentation[]
  #   layout: 'vertical' | 'horizontal' | 'grid'
  #   templates: Record<string, string>
  #   validationMessages: Record<string, string[]>
  #   focusedField: string | null
  #   submitButtonEnabled: boolean
  # }
  #
  # FORM
  # interface DataFormRenderingForm {
  #   fieldTemplateName: string | null
  #   labelTemplateName: string | null
  #   validationTemplateName: string | null
  #   actionsTemplateName: string | null
  #   columns: number
  # }
  #
  # REPRESENTATION
  # interface DataFormSlotRepresentation {
  #   slot: 'header' | 'field' | 'label' | 'input' | 'validation' | 'summary' | 'actions'
  #   propertyName: string | null
  #   contentRendered: boolean
  # }
  #
  # ACTIONS
  # type DataFormRenderingAction =
  #   | { type: 'RenderLayout'; layout: 'vertical' | 'horizontal' | 'grid' }
  #   | { type: 'RenderFieldTemplate'; propertyName: string; templateName: string }
  #   | { type: 'RenderValidationMessage'; propertyName: string }
  #   | { type: 'RenderValidationSummary' }
  #   | { type: 'RenderActionsTemplate'; templateName: string }
  #   | { type: 'RequestFieldFocus'; propertyName: string }
  #
  # REDUCER TRANSITIONS
  # RenderLayout -> layout updated
  # RenderFieldTemplate -> template rendered for field, state unchanged
  # RenderValidationMessage -> field validation slot rendered
  # RenderValidationSummary -> summary slot rendered
  # RenderActionsTemplate -> actions slot rendered
  # RequestFieldFocus -> focusedField updated
  #
  # EFFECTS
  # on RequestFieldFocus -> JS focus request for input id
  #
  # EVENTS
  # type DataFormRenderingEvent = 'DataFormLayoutRendered' | 'DataFormTemplateRendered' | 'ValidationMessageRendered' | 'ValidationSummaryRendered' | 'DataFormActionsRendered' | 'FieldFocusRequested'

  @state:DataFormRenderingState
  @SC:01
  Scenario: SC01 - Vertical layout renders labels above inputs
    @precondition:VerticalLayoutConfigured
    Given DataFormRenderingState.layout is "vertical"
    @precondition:FieldsDiscovered
    And fields include Name and Email

    @action:RenderLayout
    When DataForm renders vertical layout
    @param:layout
    And layout is "vertical"

    @postcondition:VerticalLayoutRendered
    Then each field should render label before input @UAC:01
    And fields should render in a single column @UAC:02
    And root should have data-layout="vertical" @UAC:03
    @postevent:DataFormLayoutRendered
    And DataFormLayoutRendered should include layout "vertical" @UAC:04

  @state:DataFormRenderingState
  @SC:02
  Scenario: SC02 - Grid layout renders fields in responsive columns
    @precondition:GridLayoutConfigured
    Given DataFormRenderingState.layout is "grid"
    @precondition:GridColumnsConfigured
    And DataFormRenderingForm.columns is 2

    @action:RenderLayout
    When DataForm renders grid layout
    @param:layout
    And layout is "grid"

    @postcondition:GridLayoutRendered
    Then root should have data-layout="grid" @UAC:05
    And the field container should expose two configured columns at desktop width @UAC:06
    And fields should collapse to one column at narrow width @UAC:07

  @state:DataFormRenderingState
  @SC:03
  Scenario: SC03 - Field template can replace the generated input
    @precondition:FieldTemplateProvided
    Given field "Tags" has a FieldTemplate
    @precondition:TemplateReceivesContext
    And the template expects field metadata, model, value, and validation messages

    @action:RenderFieldTemplate
    When DataForm renders the Tags field
    @param:propertyName
    And propertyName is "Tags"
    @param:templateName
    And templateName is "FieldTemplate"

    @postcondition:FieldTemplateRendered
    Then the generated default input for Tags should not render @UAC:08
    And the FieldTemplate should receive the current Tags value @UAC:09
    And the FieldTemplate should be able to report value changes back to DataForm @UAC:10
    @postevent:DataFormTemplateRendered
    And DataFormTemplateRendered should include slot "field" and propertyName "Tags" @UAC:11

  @state:DataFormRenderingState
  @SC:04
  Scenario: SC04 - Label and validation templates customize chrome without replacing input
    @precondition:ChromeTemplatesProvided
    Given field "Email" has LabelTemplate and ValidationTemplate
    @precondition:DefaultInputAvailable
    And Email uses the default text input

    @action:RenderFieldTemplate
    When DataForm renders the Email field
    @param:propertyName
    And propertyName is "Email"
    @param:templateName
    And templateName is "LabelTemplate,ValidationTemplate"

    @postcondition:ChromeTemplatesRendered
    Then the custom label template should render for Email @UAC:12
    And the default Email input should still render @UAC:13
    And the custom validation template should receive Email messages @UAC:14

  @state:DataFormRenderingState
  @SC:05
  Scenario: SC05 - Validation message markup is accessible
    @precondition:FieldHasValidationMessage
    Given validationMessages["Email"] contains "Email is required"
    @precondition:EmailFieldRendered
    And the Email input is rendered

    @action:RenderValidationMessage
    When DataForm renders Email validation messages
    @param:propertyName
    And propertyName is "Email"

    @postcondition:AccessibleErrorMarkup
    Then Email input should have aria-invalid="true" @UAC:15
    And Email input should reference the validation message id with aria-describedby @UAC:16
    And the validation message should render role="alert" or equivalent polite error semantics @UAC:17
    @postevent:ValidationMessageRendered
    And ValidationMessageRendered should include propertyName "Email" @UAC:18

  @state:DataFormRenderingState
  @SC:06
  Scenario: SC06 - Validation summary links to invalid fields
    @precondition:MultipleValidationMessages
    Given validationMessages contains Name and Email errors
    @precondition:SummaryEnabled
    And ShowValidationSummary is true

    @action:RenderValidationSummary
    When DataForm renders the validation summary

    @postcondition:SummaryRendered
    Then the summary should contain the Name error @UAC:19
    And the summary should contain the Email error @UAC:20
    And each summary item should target the corresponding field input or field group @UAC:21
    @postevent:ValidationSummaryRendered
    And ValidationSummaryRendered should include message count 2 @UAC:22

  @state:DataFormRenderingState
  @SC:07
  Scenario: SC07 - Actions template can replace default submit and reset buttons
    @precondition:ActionsTemplateProvided
    Given DataForm has an ActionsTemplate
    @precondition:DefaultActionsAvailable
    And DataForm would normally render Submit and Reset actions

    @action:RenderActionsTemplate
    When DataForm renders actions
    @param:templateName
    And templateName is "ActionsTemplate"

    @postcondition:ActionsTemplateRendered
    Then default Submit and Reset buttons should not render @UAC:23
    And the ActionsTemplate should receive submit, reset, isValidating, and isDirty context @UAC:24
    And custom submit action should invoke DataForm submission pipeline @UAC:25
    @postevent:DataFormActionsRendered
    And DataFormActionsRendered should include templateName "ActionsTemplate" @UAC:26

  @state:DataFormRenderingState
  @SC:08
  Scenario: SC08 - Focus request targets custom or generated field inputs consistently
    @precondition:InvalidFieldExists
    Given validationMessages["Password"] contains "Password is required"
    @precondition:PasswordUsesCustomTemplate
    And Password uses a FieldTemplate with a focus target id

    @action:RequestFieldFocus
    When DataForm requests focus for Password
    @param:propertyName
    And propertyName is "Password"

    @postcondition:FocusTargetResolved
    Then focusedField should be "Password" @UAC:27
    And DataForm should use the template-provided focus target when available @UAC:28
    And DataForm should fall back to the generated input id when no template target is available @UAC:29
    @postevent:FieldFocusRequested
    And FieldFocusRequested should include propertyName "Password" @UAC:30
