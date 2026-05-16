@US:05
Feature: SPEC1072.US05 - Validation Lifecycle and Submission
  As a form user
  I want DataForm to coordinate DataAnnotations, FluentValidation, conditional validators, and submit callbacks
  So that validation is predictable across field changes, blur, submit, and async rules.

  Background:
    Given the component library LowlandTech.Components.DataForm
    And DataForm can run DataAnnotations and discovered FluentValidation validators

  # STATE
  # interface DataFormValidationLifecycleState {
  #   validationMessages: Record<string, string[]>
  #   modelMessages: string[]
  #   pendingValidators: string[]
  #   isValidating: boolean
  #   isValid: boolean
  #   submitAttempted: boolean
  #   validationOrder: string[]
  # }
  #
  # FORM
  # interface DataFormValidationLifecycleForm {
  #   validateOnInput: boolean
  #   validateOnBlur: boolean
  #   validateOnSubmit: boolean
  #   stopOnFirstFailure: boolean
  #   submitMode: 'valid' | 'invalid'
  # }
  #
  # REPRESENTATION
  # interface DataFormSubmitRepresentation {
  #   editContextModelType: string
  #   isValid: boolean
  #   fieldMessages: Record<string, string[]>
  #   modelMessages: string[]
  # }
  #
  # ACTIONS
  # type DataFormValidationLifecycleAction =
  #   | { type: 'FieldInputChanged'; propertyName: string; value: unknown }
  #   | { type: 'FieldBlurred'; propertyName: string }
  #   | { type: 'RunValidationPipeline'; trigger: 'input' | 'blur' | 'submit' }
  #   | { type: 'BeginAsyncValidation'; validatorKey: string }
  #   | { type: 'CompleteAsyncValidation'; validatorKey: string; failures: string[] }
  #   | { type: 'SubmitForm' }
  #
  # REDUCER TRANSITIONS
  # FieldInputChanged -> model updated, optional validation pipeline triggered
  # FieldBlurred -> optional validation pipeline triggered
  # RunValidationPipeline -> validationOrder recorded, messages recomputed
  # BeginAsyncValidation -> pendingValidators upserted, isValidating: true
  # CompleteAsyncValidation -> pendingValidators removed, messages applied
  # SubmitForm -> submitAttempted: true, pipeline trigger submit, valid/invalid callback chosen
  #
  # EFFECTS
  # on RunValidationPipeline -> DataAnnotations first, Fluent validators second, conditional validators gated by NCalc
  # on SubmitForm -> await pending validators before callback
  #
  # EVENTS
  # type DataFormValidationLifecycleEvent = 'FieldInputChanged' | 'FieldBlurred' | 'ValidationPipelineRun' | 'AsyncValidationStarted' | 'AsyncValidationCompleted' | 'DataFormSubmitted'

  @state:DataFormValidationLifecycleState
  @SC:01
  Scenario: SC01 - Validation pipeline runs annotations before fluent validators
    @precondition:BothValidationSourcesConfigured
    Given Name has RequiredAttribute and FluentValidatorAttribute key "NameBusinessRule"
    @precondition:SubmitTrigger
    And validateOnSubmit is true

    @action:RunValidationPipeline
    When DataForm validates on submit
    @param:trigger
    And trigger is "submit"

    @postcondition:ValidationOrderRecorded
    Then validationOrder should list "DataAnnotations" before "FluentValidation" @UAC:01
    And Fluent validators should see the current model values @UAC:02
    And messages from both sources should be merged for each field @UAC:03
    @postevent:ValidationPipelineRun
    And ValidationPipelineRun should include trigger "submit" @UAC:04

  @state:DataFormValidationLifecycleState
  @SC:02
  Scenario: SC02 - Validate on input revalidates only the changed field and dependent conditional validators
    @precondition:ValidateOnInputEnabled
    Given validateOnInput is true
    @precondition:FieldChanged
    And Email changes from empty to "ada@example.com"

    @action:FieldInputChanged
    When the Email input changes
    @param:propertyName
    And propertyName is "Email"
    @param:value
    And value is "ada@example.com"

    @postcondition:InputValidationScoped
    Then DataForm should validate Email field rules @UAC:05
    And DataForm should re-evaluate conditions depending on Email @UAC:06
    And unrelated fields should not be revalidated unless their conditions depend on Email @UAC:07
    @postevent:FieldInputChanged
    And FieldInputChanged should include propertyName "Email" @UAC:08

  @state:DataFormValidationLifecycleState
  @SC:03
  Scenario: SC03 - Validate on blur delays validation until focus leaves the field
    @precondition:ValidateOnBlurEnabled
    Given validateOnBlur is true
    @precondition:ValidateOnInputDisabled
    And validateOnInput is false

    @action:FieldBlurred
    When the user edits Phone and blurs the Phone input
    @param:propertyName
    And propertyName is "Phone"

    @postcondition:BlurValidationRun
    Then validation should not run during typing @UAC:09
    And validation should run when Phone blurs @UAC:10
    And Phone messages should update after blur @UAC:11
    @postevent:FieldBlurred
    And FieldBlurred should include propertyName "Phone" @UAC:12

  @state:DataFormValidationLifecycleState
  @SC:04
  Scenario: SC04 - Async validation sets pending state and blocks submit callback
    @precondition:AsyncValidatorConfigured
    Given EmailUniqueValidator is asynchronous
    @precondition:SubmitRequested
    And the user submits the form

    @action:BeginAsyncValidation
    When EmailUniqueValidator starts
    @param:validatorKey
    And validatorKey is "EmailUnique"

    @postcondition:PendingValidationState
    Then pendingValidators should contain "EmailUnique" @UAC:13
    And isValidating should be true @UAC:14
    And OnValidSubmit and OnInvalidSubmit should wait for completion @UAC:15
    @postevent:AsyncValidationStarted
    And AsyncValidationStarted should include validatorKey "EmailUnique" @UAC:16

  @state:DataFormValidationLifecycleState
  @SC:05
  Scenario: SC05 - Async validation completion updates messages and resumes submit decision
    @precondition:AsyncValidationPending
    Given pendingValidators contains "EmailUnique"
    @precondition:AsyncValidatorReturnsFailure
    And EmailUniqueValidator returns "Email is already used"

    @action:CompleteAsyncValidation
    When EmailUniqueValidator completes
    @param:validatorKey
    And validatorKey is "EmailUnique"
    @param:failures
    And failures contains "Email is already used"

    @postcondition:AsyncFailureApplied
    Then pendingValidators should not contain "EmailUnique" @UAC:17
    And validationMessages["Email"] should contain "Email is already used" @UAC:18
    And isValid should be false @UAC:19
    And OnInvalidSubmit should be invoked for the submit attempt @UAC:20
    @postevent:AsyncValidationCompleted
    And AsyncValidationCompleted should include failure count 1 @UAC:21

  @state:DataFormValidationLifecycleState
  @SC:06
  Scenario: SC06 - Stop on first failure prevents expensive fluent validator after annotation error
    @precondition:StopOnFirstFailureEnabled
    Given stopOnFirstFailure is true
    @precondition:AnnotationFailureExists
    And Email fails RequiredAttribute

    @action:RunValidationPipeline
    When DataForm validates Email
    @param:trigger
    And trigger is "submit"

    @postcondition:PipelineStopped
    Then Email RequiredAttribute message should be applied @UAC:22
    And EmailUniqueValidator should not run @UAC:23
    And validationOrder should record that FluentValidation was skipped for Email @UAC:24

  @state:DataFormValidationLifecycleState
  @SC:07
  Scenario: SC07 - Valid submit includes the final EditContext and model
    @precondition:AllValidatorsPass
    Given DataAnnotations and Fluent validators return no failures
    @precondition:SubmitRequested
    And the user submits the form

    @action:SubmitForm
    When DataForm completes the validation pipeline

    @postcondition:ValidSubmitRaised
    Then isValid should be true @UAC:25
    And OnValidSubmit should receive the EditContext @UAC:26
    And OnInvalidSubmit should not be invoked @UAC:27
    @postevent:DataFormSubmitted
    And DataFormSubmitted should include isValid true @UAC:28

  @state:DataFormValidationLifecycleState
  @SC:08
  Scenario: SC08 - Invalid submit focuses the first invalid field when configured
    @precondition:InvalidSubmitFocusEnabled
    Given FocusFirstInvalidField is true
    @precondition:MultipleErrorsExist
    And validationMessages contains errors for Email and Name in render order Name then Email

    @action:SubmitForm
    When DataForm handles invalid submit

    @postcondition:FirstInvalidFieldFocused
    Then OnInvalidSubmit should receive the EditContext @UAC:29
    And DataForm should request focus for the Name input @UAC:30
    And the validation summary should link to Name and Email errors @UAC:31
    @postevent:DataFormSubmitted
    And DataFormSubmitted should include isValid false @UAC:32
