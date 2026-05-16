@US:01
Feature: SPEC1072.US01 - Model Binding and Field Generation
  As a component consumer
  I want DataForm to accept a model and generate an EditContext-backed form surface
  So that common CRUD forms can be built from model metadata without hand-wiring every field.

  Background:
    Given the component library LowlandTech.Components.DataForm
    And DataForm is rendered with a non-null model instance

  # STATE
  # interface DataFormModelBindingState {
  #   modelType: string
  #   model: Record<string, unknown>
  #   editContextCreated: boolean
  #   fields: DataFormFieldRepresentation[]
  #   excludedProperties: string[]
  #   readOnlyProperties: string[]
  #   dirtyFields: string[]
  #   validationMessages: Record<string, string[]>
  # }
  #
  # FORM
  # interface DataFormModelBindingForm {
  #   model: unknown
  #   includeProperties: string[] | null
  #   excludeProperties: string[]
  #   readOnly: boolean
  #   layout: 'vertical' | 'horizontal' | 'grid'
  # }
  #
  # REPRESENTATION
  # interface DataFormFieldRepresentation {
  #   propertyName: string
  #   displayName: string
  #   propertyType: string
  #   inputKind: 'text' | 'number' | 'date' | 'checkbox' | 'select' | 'textarea' | 'custom'
  #   required: boolean
  #   readOnly: boolean
  #   order: number
  # }
  #
  # ACTIONS
  # type DataFormModelBindingAction =
  #   | { type: 'InitializeForm'; model: unknown }
  #   | { type: 'DiscoverFields'; modelType: string }
  #   | { type: 'UpdateFieldValue'; propertyName: string; value: unknown }
  #   | { type: 'ResetModel'; model: unknown }
  #   | { type: 'RenderField'; propertyName: string }
  #
  # REDUCER TRANSITIONS
  # InitializeForm -> model, modelType, editContextCreated: true
  # DiscoverFields -> fields populated from public settable properties and metadata
  # UpdateFieldValue -> model[propertyName], dirtyFields upserted
  # ResetModel -> model replaced, dirtyFields cleared, validationMessages cleared
  # RenderField -> view emitted for field, state unchanged
  #
  # EFFECTS
  # on InitializeForm -> create EditContext(model)
  # on UpdateFieldValue -> EditContext.NotifyFieldChanged(fieldIdentifier)
  #
  # EVENTS
  # type DataFormModelBindingEvent = 'DataFormInitialized' | 'FieldsDiscovered' | 'FieldValueChanged' | 'ModelReset' | 'FieldRendered'

  @state:DataFormModelBindingState
  @SC:01
  Scenario: SC01 - DataForm creates an EditContext for the supplied model
    @precondition:ModelProvided
    Given DataFormModelBindingForm.model is a CustomerFormModel instance
    @precondition:NoEditContextProvided
    And no external EditContext is supplied

    @action:InitializeForm
    When DataForm initializes
    @param:model
    And model is the CustomerFormModel instance

    @postcondition:EditContextCreated
    Then DataFormModelBindingState.editContextCreated should be true @UAC:01
    And the EditContext model should be the supplied CustomerFormModel instance @UAC:02
    And DataForm should cascade the EditContext to child field components @UAC:03
    @postevent:DataFormInitialized
    And DataFormInitialized should include modelType "CustomerFormModel" @UAC:04

  @state:DataFormModelBindingState
  @SC:02
  Scenario: SC02 - DataForm discovers public editable properties
    @precondition:ModelTypeKnown
    Given CustomerFormModel has public properties Name, Email, IsActive, CreditLimit, and CreatedOn
    @precondition:PropertiesEditable
    And those properties have public getters and setters

    @action:DiscoverFields
    When DataForm discovers fields for CustomerFormModel
    @param:modelType
    And modelType is "CustomerFormModel"

    @postcondition:FieldsDiscovered
    Then fields should contain Name, Email, IsActive, CreditLimit, and CreatedOn @UAC:05
    And fields should not contain methods or backing fields @UAC:06
    And fields should preserve a deterministic order by metadata order then declaration order @UAC:07
    @postevent:FieldsDiscovered
    And FieldsDiscovered should include the field count @UAC:08

  @state:DataFormModelBindingState
  @SC:03
  Scenario: SC03 - DataForm maps property types to default input kinds
    @precondition:FieldsDiscovered
    Given fields include string, numeric, boolean, enum, DateTime, and nullable types

    @action:RenderField
    When DataForm renders generated fields
    @param:propertyName
    And propertyName is each discovered property

    @postcondition:InputKindsMapped
    Then string properties should render text inputs by default @UAC:09
    And numeric properties should render number inputs by default @UAC:10
    And boolean properties should render checkbox or switch inputs by default @UAC:11
    And enum properties should render select inputs by default @UAC:12
    And DateTime properties should render date or datetime inputs by default @UAC:13
    And nullable properties should allow empty values @UAC:14

  @state:DataFormModelBindingState
  @SC:04
  Scenario: SC04 - Display metadata controls labels and order
    @precondition:DisplayAttributesPresent
    Given CustomerFormModel.Name has Display Name "Customer name" and Order 10
    @precondition:AnotherDisplayAttributePresent
    And CustomerFormModel.Email has Display Name "Email address" and Order 20

    @action:DiscoverFields
    When DataForm discovers field metadata
    @param:modelType
    And modelType is "CustomerFormModel"

    @postcondition:DisplayMetadataApplied
    Then the Name field displayName should be "Customer name" @UAC:15
    And the Email field displayName should be "Email address" @UAC:16
    And the Name field should render before the Email field @UAC:17

  @state:DataFormModelBindingState
  @SC:05
  Scenario: SC05 - Include and exclude lists shape generated fields
    @precondition:IncludeExcludeConfigured
    Given includeProperties contains Name, Email, and InternalCode
    @precondition:ExcludedPropertyConfigured
    And excludeProperties contains InternalCode

    @action:DiscoverFields
    When DataForm discovers fields with include and exclude lists
    @param:modelType
    And modelType is "CustomerFormModel"

    @postcondition:FieldListShaped
    Then fields should contain Name and Email @UAC:18
    And fields should not contain InternalCode @UAC:19
    And fields should not contain properties outside includeProperties @UAC:20

  @state:DataFormModelBindingState
  @SC:06
  Scenario: SC06 - Updating a field writes to the model and notifies EditContext
    @precondition:FormInitialized
    Given DataForm is bound to CustomerFormModel with Name "Old name"
    @precondition:FieldRendered
    And the Name field is rendered

    @action:UpdateFieldValue
    When the user changes Name to "New name"
    @param:propertyName
    And propertyName is "Name"
    @param:value
    And value is "New name"

    @postcondition:ModelUpdated
    Then CustomerFormModel.Name should be "New name" @UAC:21
    And dirtyFields should contain "Name" @UAC:22
    And EditContext should be notified for the Name field @UAC:23
    @postevent:FieldValueChanged
    And FieldValueChanged should include propertyName "Name" and value "New name" @UAC:24

  @state:DataFormModelBindingState
  @SC:07
  Scenario: SC07 - Resetting the model recreates field state without stale messages
    @precondition:FormDirty
    Given dirtyFields contains Name and Email
    @precondition:ValidationMessagesExist
    And validationMessages contains an Email error

    @action:ResetModel
    When DataForm receives a new CustomerFormModel instance
    @param:model
    And model is the new CustomerFormModel instance

    @postcondition:ModelStateReset
    Then the EditContext should point at the new model instance @UAC:25
    And dirtyFields should be empty @UAC:26
    And validationMessages should be empty @UAC:27
    @postevent:ModelReset
    And ModelReset should include the new modelType @UAC:28

  @state:DataFormModelBindingState
  @SC:08
  Scenario: SC08 - Read-only mode renders values without editable inputs
    @precondition:ReadOnlyModeEnabled
    Given DataFormModelBindingForm.readOnly is true
    @precondition:FieldsDiscovered
    And fields include Name and Email

    @action:RenderField
    When DataForm renders fields in read-only mode
    @param:propertyName
    And propertyName is "Name"

    @postcondition:ReadOnlyFieldsRendered
    Then the Name field should render its current value @UAC:29
    And the Name field should not render an editable input @UAC:30
    And validation should not mark read-only fields dirty on render @UAC:31
