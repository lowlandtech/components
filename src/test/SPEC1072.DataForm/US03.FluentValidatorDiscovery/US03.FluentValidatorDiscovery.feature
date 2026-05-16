@US:03
Feature: SPEC1072.US03 - Fluent Validator Discovery
  As a form author
  I want FluentValidatorAttribute to resolve named validators from ValidatorDiscoveryService
  So that DataForm can run reusable FluentValidation rules against a property or a whole model.

  Background:
    Given the component library LowlandTech.Components.DataForm
    And FluentValidation validators are registered in dependency injection
    And ValidatorDiscoveryService can resolve validators by string key or enum key

  # STATE
  # interface DataFormFluentValidationState {
  #   model: Record<string, unknown>
  #   validatorAttributes: FluentValidatorAttributeRepresentation[]
  #   resolvedValidators: ResolvedValidatorRepresentation[]
  #   validationMessages: Record<string, string[]>
  #   modelMessages: string[]
  #   discoveryErrors: string[]
  # }
  #
  # FORM
  # interface FluentValidatorAttributeForm {
  #   key: string
  #   enumKey: string | null
  #   scope: 'property' | 'model'
  #   condition: string | null
  #   severity: 'error' | 'warning'
  # }
  #
  # REPRESENTATION
  # interface FluentValidatorAttributeRepresentation {
  #   memberName: string | null
  #   key: string
  #   scope: 'property' | 'model'
  #   condition: string | null
  # }
  #
  # interface ResolvedValidatorRepresentation {
  #   key: string
  #   validatorType: string
  #   targetType: string
  #   scope: 'property' | 'model'
  # }
  #
  # ACTIONS
  # type DataFormFluentValidationAction =
  #   | { type: 'DiscoverValidatorAttributes'; modelType: string }
  #   | { type: 'ResolveValidator'; key: string; scope: 'property' | 'model' }
  #   | { type: 'RunPropertyValidator'; propertyName: string; key: string }
  #   | { type: 'RunModelValidator'; key: string }
  #   | { type: 'ApplyFluentValidationFailures'; failures: FluentValidationFailureRepresentation[] }
  #
  # REDUCER TRANSITIONS
  # DiscoverValidatorAttributes -> validatorAttributes populated from model and properties
  # ResolveValidator -> resolvedValidators upserted or discoveryErrors appended
  # RunPropertyValidator -> property failures mapped to validationMessages[propertyName]
  # RunModelValidator -> model and property failures mapped by failure.PropertyName
  # ApplyFluentValidationFailures -> validationMessages/modelMessages updated
  #
  # EFFECTS
  # on ResolveValidator -> ValidatorDiscoveryService.Resolve(key)
  # on RunPropertyValidator -> FluentValidation property validation invoked with property value and model context
  # on RunModelValidator -> FluentValidation model validation invoked with whole model
  #
  # EVENTS
  # type DataFormFluentValidationEvent = 'ValidatorAttributesDiscovered' | 'ValidatorResolved' | 'FluentValidationRun' | 'FluentValidationFailuresApplied' | 'ValidatorDiscoveryFailed'

  @state:DataFormFluentValidationState
  @SC:01
  Scenario: SC01 - Attribute with string key resolves a property validator
    @precondition:PropertyAttributePresent
    Given CustomerFormModel.Email has FluentValidatorAttribute key "EmailUnique"
    @precondition:DiscoveryServiceHasValidator
    And ValidatorDiscoveryService can resolve "EmailUnique" to EmailUniqueValidator

    @action:ResolveValidator
    When DataForm resolves the Email validator
    @param:key
    And key is "EmailUnique"
    @param:scope
    And scope is "property"

    @postcondition:PropertyValidatorResolved
    Then resolvedValidators should contain key "EmailUnique" @UAC:01
    And the resolved validator targetType should match the Email property type or supported adapter type @UAC:02
    @postevent:ValidatorResolved
    And ValidatorResolved should include validatorType "EmailUniqueValidator" @UAC:03

  @state:DataFormFluentValidationState
  @SC:02
  Scenario: SC02 - Attribute with enum key resolves a validator through enum name
    @precondition:EnumAttributePresent
    Given CustomerFormModel.TaxId has FluentValidatorAttribute enum key CustomerValidators.TaxId
    @precondition:DiscoveryServiceHasEnumMapping
    And ValidatorDiscoveryService maps CustomerValidators.TaxId to TaxIdValidator

    @action:ResolveValidator
    When DataForm resolves the TaxId validator
    @param:key
    And key is "CustomerValidators.TaxId"
    @param:scope
    And scope is "property"

    @postcondition:EnumValidatorResolved
    Then resolvedValidators should contain key "CustomerValidators.TaxId" @UAC:04
    And the resolved validator should be TaxIdValidator @UAC:05
    And discovery should not depend on localized display text @UAC:06

  @state:DataFormFluentValidationState
  @SC:03
  Scenario: SC03 - Property validator failure maps to the property message store
    @precondition:PropertyValidatorResolved
    Given EmailUniqueValidator is resolved for Email
    @precondition:ValidatorFails
    And CustomerFormModel.Email is already used

    @action:RunPropertyValidator
    When DataForm runs the EmailUnique validator
    @param:propertyName
    And propertyName is "Email"
    @param:key
    And key is "EmailUnique"

    @postcondition:PropertyFailureMapped
    Then validationMessages["Email"] should contain the FluentValidation failure message @UAC:07
    And the Email field should render aria-invalid="true" @UAC:08
    @postevent:FluentValidationRun
    And FluentValidationRun should include propertyName "Email" @UAC:09

  @state:DataFormFluentValidationState
  @SC:04
  Scenario: SC04 - Model validator can add property and model-level failures
    @precondition:ModelAttributePresent
    Given CustomerFormModel has FluentValidatorAttribute key "CustomerBusinessRules"
    @precondition:ModelValidatorFails
    And the model violates a cross-field business rule

    @action:RunModelValidator
    When DataForm runs CustomerBusinessRules
    @param:key
    And key is "CustomerBusinessRules"

    @postcondition:ModelFailuresMapped
    Then failures with PropertyName should map to that field's validationMessages @UAC:10
    And failures without PropertyName should map to modelMessages @UAC:11
    And validation summary should include model-level failures @UAC:12
    @postevent:FluentValidationFailuresApplied
    And FluentValidationFailuresApplied should include failure count @UAC:13

  @state:DataFormFluentValidationState
  @SC:05
  Scenario: SC05 - Multiple validators on one property run in deterministic order
    @precondition:MultipleAttributesPresent
    Given CustomerFormModel.Email has FluentValidatorAttribute keys "EmailFormatPlus" and "EmailUnique"
    @precondition:BothValidatorsRegistered
    And ValidatorDiscoveryService can resolve both validators

    @action:RunPropertyValidator
    When DataForm validates Email
    @param:propertyName
    And propertyName is "Email"
    @param:key
    And key is "EmailFormatPlus,EmailUnique"

    @postcondition:ValidatorsRunInOrder
    Then EmailFormatPlus should run before EmailUnique @UAC:14
    And validationMessages["Email"] should preserve failure order @UAC:15
    And DataForm should support either aggregate failure reporting or configured cascade stop @UAC:16

  @state:DataFormFluentValidationState
  @SC:06
  Scenario: SC06 - Missing validator key produces a discovery error without crashing
    @precondition:UnknownValidatorAttributePresent
    Given CustomerFormModel.LegacyCode has FluentValidatorAttribute key "MissingValidator"
    @precondition:DiscoveryServiceCannotResolve
    And ValidatorDiscoveryService has no validator for "MissingValidator"

    @action:ResolveValidator
    When DataForm resolves validators for LegacyCode
    @param:key
    And key is "MissingValidator"
    @param:scope
    And scope is "property"

    @postcondition:DiscoveryErrorRecorded
    Then discoveryErrors should contain "MissingValidator" @UAC:17
    And DataForm should continue rendering the LegacyCode field @UAC:18
    And validation summary should include a diagnostic message when diagnostics are enabled @UAC:19
    @postevent:ValidatorDiscoveryFailed
    And ValidatorDiscoveryFailed should include key "MissingValidator" @UAC:20

  @state:DataFormFluentValidationState
  @SC:07
  Scenario: SC07 - Validator receives model context for property validation
    @precondition:PropertyValidatorRequiresContext
    Given DiscountValidator validates Discount using CustomerType from the model
    @precondition:ContextModelAvailable
    And CustomerFormModel.CustomerType is "Enterprise"

    @action:RunPropertyValidator
    When DataForm runs DiscountValidator for Discount
    @param:propertyName
    And propertyName is "Discount"
    @param:key
    And key is "DiscountByCustomerType"

    @postcondition:ModelContextPassed
    Then the validator should receive the Discount property value @UAC:21
    And the validator should be able to read CustomerType from validation context @UAC:22
    And the resulting failure should map to Discount when invalid @UAC:23

  @state:DataFormFluentValidationState
  @SC:08
  Scenario: SC08 - Async FluentValidation rules are awaited before submit completes
    @precondition:AsyncValidatorResolved
    Given EmailUniqueValidator uses an asynchronous uniqueness check
    @precondition:SubmitRequested
    And the user submits DataForm

    @action:RunPropertyValidator
    When DataForm runs EmailUniqueValidator
    @param:propertyName
    And propertyName is "Email"
    @param:key
    And key is "EmailUnique"

    @postcondition:AsyncValidationAwaited
    Then DataForm should await the validator result before computing isValid @UAC:24
    And OnValidSubmit should not fire until async validators complete @UAC:25
    And validation state should indicate pending validation while the task is running @UAC:26
    @postevent:FluentValidationRun
    And FluentValidationRun should include async=true @UAC:27
