@US:02
Feature: SPEC1072.US02 - DataAnnotations Validation
  As a form author
  I want DataForm to run standard DataAnnotations against properties and the model
  So that existing .NET validation metadata works automatically in generated forms.

  Background:
    Given the component library LowlandTech.Components.DataForm
    And the model uses System.ComponentModel.DataAnnotations attributes

  # STATE
  # interface DataFormAnnotationsState {
  #   model: Record<string, unknown>
  #   validationMessages: Record<string, string[]>
  #   validationSummary: string[]
  #   validationMode: 'field' | 'model' | 'submit'
  #   isValid: boolean
  #   lastValidatedField: string | null
  # }
  #
  # FORM
  # interface DataFormAnnotationsForm {
  #   propertyName: string | null
  #   value: unknown
  #   validateOnInput: boolean
  #   validateOnBlur: boolean
  #   validateOnSubmit: boolean
  # }
  #
  # REPRESENTATION
  # interface DataAnnotationResultRepresentation {
  #   memberName: string | null
  #   message: string
  #   source: 'DataAnnotations'
  #   severity: 'error'
  # }
  #
  # ACTIONS
  # type DataFormAnnotationsAction =
  #   | { type: 'ValidateFieldAnnotations'; propertyName: string }
  #   | { type: 'ValidateModelAnnotations' }
  #   | { type: 'ApplyAnnotationMessages'; messages: DataAnnotationResultRepresentation[] }
  #   | { type: 'ClearFieldMessages'; propertyName: string }
  #   | { type: 'SubmitForm' }
  #
  # REDUCER TRANSITIONS
  # ValidateFieldAnnotations -> lastValidatedField set, field messages recomputed
  # ValidateModelAnnotations -> all annotation messages recomputed
  # ApplyAnnotationMessages -> validationMessages and validationSummary updated
  # ClearFieldMessages -> validationMessages[propertyName] removed
  # SubmitForm -> ValidateModelAnnotations, isValid updated
  #
  # EFFECTS
  # on ValidateFieldAnnotations -> Validator.TryValidateProperty
  # on ValidateModelAnnotations -> Validator.TryValidateObject(validateAllProperties: true)
  #
  # EVENTS
  # type DataFormAnnotationsEvent = 'FieldAnnotationsValidated' | 'ModelAnnotationsValidated' | 'AnnotationMessagesChanged' | 'FormSubmitValidated'

  @state:DataFormAnnotationsState
  @SC:01
  Scenario: SC01 - Required attribute produces a field message
    @precondition:RequiredAttributePresent
    Given CustomerFormModel.Name has RequiredAttribute
    @precondition:EmptyValue
    And CustomerFormModel.Name is empty

    @action:ValidateFieldAnnotations
    When DataForm validates the Name field
    @param:propertyName
    And propertyName is "Name"

    @postcondition:RequiredMessageAdded
    Then validationMessages["Name"] should contain the RequiredAttribute message @UAC:01
    And lastValidatedField should be "Name" @UAC:02
    And isValid should be false @UAC:03
    @postevent:FieldAnnotationsValidated
    And FieldAnnotationsValidated should include propertyName "Name" @UAC:04

  @state:DataFormAnnotationsState
  @SC:02
  Scenario: SC02 - StringLength attribute validates maximum length
    @precondition:StringLengthAttributePresent
    Given CustomerFormModel.Name has StringLength maximum 20
    @precondition:TooLongValue
    And CustomerFormModel.Name is 21 characters long

    @action:ValidateFieldAnnotations
    When DataForm validates the Name field
    @param:propertyName
    And propertyName is "Name"

    @postcondition:LengthMessageAdded
    Then validationMessages["Name"] should contain the StringLength message @UAC:05
    And the Name field should render aria-invalid="true" @UAC:06
    And the Name field error text should be associated with the input by aria-describedby @UAC:07

  @state:DataFormAnnotationsState
  @SC:03
  Scenario: SC03 - Range attribute validates numeric values
    @precondition:RangeAttributePresent
    Given CustomerFormModel.CreditLimit has Range 0 to 10000
    @precondition:OutOfRangeValue
    And CustomerFormModel.CreditLimit is 20000

    @action:ValidateFieldAnnotations
    When DataForm validates the CreditLimit field
    @param:propertyName
    And propertyName is "CreditLimit"

    @postcondition:RangeMessageAdded
    Then validationMessages["CreditLimit"] should contain the Range message @UAC:08
    And the CreditLimit input should remain editable @UAC:09
    And the invalid numeric value should remain visible for correction @UAC:10

  @state:DataFormAnnotationsState
  @SC:04
  Scenario: SC04 - ValidationAttribute using ValidationContext receives services
    @precondition:ServiceAwareAttributePresent
    Given CustomerFormModel.AccountNumber has a custom ValidationAttribute
    @precondition:ServiceProviderAvailable
    And the attribute requests a service from ValidationContext

    @action:ValidateFieldAnnotations
    When DataForm validates AccountNumber
    @param:propertyName
    And propertyName is "AccountNumber"

    @postcondition:ValidationContextServicesAvailable
    Then ValidationContext should expose the DataForm service provider @UAC:11
    And the custom attribute should be able to resolve its service @UAC:12
    And validationMessages should include any returned custom message @UAC:13

  @state:DataFormAnnotationsState
  @SC:05
  Scenario: SC05 - IValidatableObject model errors appear in summary and field messages
    @precondition:ModelImplementsIValidatableObject
    Given CustomerFormModel implements IValidatableObject
    @precondition:CrossFieldInvalid
    And StartDate is after EndDate

    @action:ValidateModelAnnotations
    When DataForm validates the whole model

    @postcondition:ModelValidationApplied
    Then validationSummary should contain the cross-field error @UAC:14
    And field messages should be attached to member names returned by the result @UAC:15
    And model-level messages without member names should appear in the summary @UAC:16
    @postevent:ModelAnnotationsValidated
    And ModelAnnotationsValidated should include result count @UAC:17

  @state:DataFormAnnotationsState
  @SC:06
  Scenario: SC06 - Correcting a field clears stale annotation messages
    @precondition:FieldHasError
    Given validationMessages["Email"] contains "Email is not valid"
    @precondition:CorrectedValue
    And CustomerFormModel.Email is "ada@example.com"

    @action:ValidateFieldAnnotations
    When DataForm revalidates Email
    @param:propertyName
    And propertyName is "Email"

    @postcondition:FieldMessagesCleared
    Then validationMessages should not contain "Email" @UAC:18
    And Email should render aria-invalid="false" @UAC:19
    @postevent:AnnotationMessagesChanged
    And AnnotationMessagesChanged should include removed field "Email" @UAC:20

  @state:DataFormAnnotationsState
  @SC:07
  Scenario: SC07 - Submit validates all annotation rules before valid submit callback
    @precondition:SubmitRequested
    Given validateOnSubmit is true
    @precondition:ModelContainsAnnotationErrors
    And CustomerFormModel has one invalid property

    @action:SubmitForm
    When the user submits DataForm

    @postcondition:InvalidSubmitBlocked
    Then ValidateModelAnnotations should run before OnValidSubmit @UAC:21
    And OnValidSubmit should not be invoked @UAC:22
    And OnInvalidSubmit should be invoked with the EditContext @UAC:23
    @postevent:FormSubmitValidated
    And FormSubmitValidated should include isValid false @UAC:24

  @state:DataFormAnnotationsState
  @SC:08
  Scenario: SC08 - Valid annotation model allows valid submit callback
    @precondition:SubmitRequested
    Given validateOnSubmit is true
    @precondition:ModelAnnotationValid
    And CustomerFormModel satisfies all DataAnnotations

    @action:SubmitForm
    When the user submits DataForm

    @postcondition:ValidSubmitAllowed
    Then validationMessages should be empty @UAC:25
    And isValid should be true @UAC:26
    And OnValidSubmit should be invoked with the EditContext @UAC:27
    @postevent:FormSubmitValidated
    And FormSubmitValidated should include isValid true @UAC:28
