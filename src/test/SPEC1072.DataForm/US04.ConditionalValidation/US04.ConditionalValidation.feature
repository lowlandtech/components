@US:04
Feature: SPEC1072.US04 - Conditional Fluent Validation with NCalc
  As a form author
  I want FluentValidatorAttribute to include an optional Condition expression
  So that dynamic validators run only when model state satisfies an NCalc expression.

  Background:
    Given the component library LowlandTech.Components.DataForm
    And FluentValidatorAttribute supports a Condition string
    And NCalc is the expression engine for evaluating conditions

  # STATE
  # interface DataFormConditionalValidationState {
  #   model: Record<string, unknown>
  #   conditionExpression: string | null
  #   conditionParameters: Record<string, unknown>
  #   conditionResult: boolean | null
  #   validatorsRun: string[]
  #   validatorsSkipped: string[]
  #   conditionErrors: string[]
  # }
  #
  # FORM
  # interface DataFormConditionForm {
  #   key: string
  #   condition: string | null
  #   propertyName: string | null
  #   model: unknown
  # }
  #
  # REPRESENTATION
  # interface DataFormConditionRepresentation {
  #   expression: string
  #   parameters: Record<string, unknown>
  #   result: boolean
  #   skippedReason: string | null
  # }
  #
  # ACTIONS
  # type DataFormConditionalValidationAction =
  #   | { type: 'BuildConditionParameters'; model: unknown; propertyName: string | null }
  #   | { type: 'EvaluateValidatorCondition'; key: string; condition: string | null }
  #   | { type: 'RunConditionalValidator'; key: string; propertyName: string | null }
  #   | { type: 'SkipConditionalValidator'; key: string; reason: string }
  #   | { type: 'RecordConditionError'; key: string; message: string }
  #
  # REDUCER TRANSITIONS
  # BuildConditionParameters -> conditionParameters from model fields and special values
  # EvaluateValidatorCondition -> conditionResult true/false or conditionErrors
  # RunConditionalValidator -> validatorsRun upserted
  # SkipConditionalValidator -> validatorsSkipped upserted
  # RecordConditionError -> conditionErrors appended
  #
  # EFFECTS
  # on EvaluateValidatorCondition -> new NCalc.Expression(condition).Evaluate(parameters)
  # on RunConditionalValidator -> resolve and run FluentValidation validator
  #
  # EVENTS
  # type DataFormConditionalValidationEvent = 'ConditionParametersBuilt' | 'ValidatorConditionEvaluated' | 'ConditionalValidatorRun' | 'ConditionalValidatorSkipped' | 'ConditionEvaluationFailed'

  @state:DataFormConditionalValidationState
  @SC:01
  Scenario: SC01 - Null condition runs the validator
    @precondition:AttributeHasNoCondition
    Given FluentValidatorAttribute key "EmailUnique" has Condition null
    @precondition:ValidatorResolved
    And ValidatorDiscoveryService resolves "EmailUnique"

    @action:EvaluateValidatorCondition
    When DataForm evaluates the validator condition
    @param:key
    And key is "EmailUnique"
    @param:condition
    And condition is null

    @postcondition:ConditionDefaultsToTrue
    Then conditionResult should be true @UAC:01
    And validatorsSkipped should not contain "EmailUnique" @UAC:02
    @postevent:ValidatorConditionEvaluated
    And ValidatorConditionEvaluated should include result true @UAC:03

  @state:DataFormConditionalValidationState
  @SC:02
  Scenario: SC02 - Boolean property condition runs validator when true
    @precondition:ConditionConfigured
    Given FluentValidatorAttribute key "VatNumberRequired" has Condition "IsBusiness == true"
    @precondition:ModelMatchesCondition
    And CustomerFormModel.IsBusiness is true

    @action:RunConditionalValidator
    When DataForm validates VatNumber
    @param:key
    And key is "VatNumberRequired"
    @param:propertyName
    And propertyName is "VatNumber"

    @postcondition:ConditionalValidatorRuns
    Then conditionResult should be true @UAC:04
    And validatorsRun should contain "VatNumberRequired" @UAC:05
    And VatNumber validation failures should be applied when the validator fails @UAC:06
    @postevent:ConditionalValidatorRun
    And ConditionalValidatorRun should include key "VatNumberRequired" @UAC:07

  @state:DataFormConditionalValidationState
  @SC:03
  Scenario: SC03 - Boolean property condition skips validator when false
    @precondition:ConditionConfigured
    Given FluentValidatorAttribute key "VatNumberRequired" has Condition "IsBusiness == true"
    @precondition:ModelDoesNotMatchCondition
    And CustomerFormModel.IsBusiness is false

    @action:SkipConditionalValidator
    When DataForm validates VatNumber
    @param:key
    And key is "VatNumberRequired"
    @param:reason
    And reason is "Condition evaluated to false"

    @postcondition:ConditionalValidatorSkipped
    Then conditionResult should be false @UAC:08
    And validatorsSkipped should contain "VatNumberRequired" @UAC:09
    And no VatNumber failure should be added by this validator @UAC:10
    @postevent:ConditionalValidatorSkipped
    And ConditionalValidatorSkipped should include reason "Condition evaluated to false" @UAC:11

  @state:DataFormConditionalValidationState
  @SC:04
  Scenario: SC04 - Expression supports arithmetic and comparison operators
    @precondition:ArithmeticConditionConfigured
    Given FluentValidatorAttribute key "HighCreditApproval" has Condition "CreditLimit > 10000 and Age >= 18"
    @precondition:ModelMatchesArithmeticCondition
    And CreditLimit is 25000 and Age is 35

    @action:EvaluateValidatorCondition
    When DataForm evaluates the condition
    @param:key
    And key is "HighCreditApproval"
    @param:condition
    And condition is "CreditLimit > 10000 and Age >= 18"

    @postcondition:ArithmeticConditionTrue
    Then conditionResult should be true @UAC:12
    And conditionParameters should include CreditLimit and Age @UAC:13
    And NCalc should evaluate numeric operands as numbers, not strings @UAC:14

  @state:DataFormConditionalValidationState
  @SC:05
  Scenario: SC05 - Expression supports string operators and null checks
    @precondition:StringConditionConfigured
    Given FluentValidatorAttribute key "PostalCodeRequired" has Condition "Country == 'US' and PostalCode == null"
    @precondition:ModelMatchesStringCondition
    And Country is "US" and PostalCode is null

    @action:EvaluateValidatorCondition
    When DataForm evaluates the condition
    @param:key
    And key is "PostalCodeRequired"
    @param:condition
    And condition is "Country == 'US' and PostalCode == null"

    @postcondition:StringConditionTrue
    Then conditionResult should be true @UAC:15
    And conditionParameters should include Country and PostalCode @UAC:16
    And null operands should be passed to NCalc as null values @UAC:17

  @state:DataFormConditionalValidationState
  @SC:06
  Scenario: SC06 - Expression can reference current property value through special operand
    @precondition:PropertyValueConditionConfigured
    Given FluentValidatorAttribute key "StrongPassword" has Condition "Value != null and Length(Value) > 0"
    @precondition:PropertyValueAvailable
    And Password value is "abc"

    @action:BuildConditionParameters
    When DataForm builds condition parameters for Password
    @param:propertyName
    And propertyName is "Password"
    @param:model
    And model is CustomerFormModel

    @postcondition:SpecialValueOperandAvailable
    Then conditionParameters should include Value equal to "abc" @UAC:18
    And conditionParameters should include PropertyName equal to "Password" @UAC:19
    And the condition should evaluate to true @UAC:20
    @postevent:ConditionParametersBuilt
    And ConditionParametersBuilt should include propertyName "Password" @UAC:21

  @state:DataFormConditionalValidationState
  @SC:07
  Scenario: SC07 - Invalid condition records diagnostic and skips validator by default
    @precondition:InvalidConditionConfigured
    Given FluentValidatorAttribute key "BrokenRule" has Condition "IsBusiness =="
    @precondition:ValidatorResolved
    And ValidatorDiscoveryService resolves "BrokenRule"

    @action:RecordConditionError
    When DataForm evaluates the invalid condition
    @param:key
    And key is "BrokenRule"
    @param:message
    And message is the NCalc parse error

    @postcondition:ConditionErrorRecorded
    Then conditionErrors should contain a diagnostic for "BrokenRule" @UAC:22
    And validatorsSkipped should contain "BrokenRule" @UAC:23
    And no validation failure from BrokenRule should be applied @UAC:24
    @postevent:ConditionEvaluationFailed
    And ConditionEvaluationFailed should include key and parse error @UAC:25

  @state:DataFormConditionalValidationState
  @SC:08
  Scenario: SC08 - Condition re-evaluates when dependent field changes
    @precondition:DependencyTracked
    Given FluentValidatorAttribute key "VatNumberRequired" has Condition "IsBusiness == true"
    @precondition:InitialConditionFalse
    And IsBusiness is false

    @action:EvaluateValidatorCondition
    When the user changes IsBusiness to true
    @param:key
    And key is "VatNumberRequired"
    @param:condition
    And condition is "IsBusiness == true"

    @postcondition:ConditionReevaluated
    Then DataForm should re-evaluate conditions referencing IsBusiness @UAC:26
    And conditionResult should change from false to true @UAC:27
    And VatNumberRequired should run on the next configured validation trigger @UAC:28
    @postevent:ValidatorConditionEvaluated
    And ValidatorConditionEvaluated should include dependency "IsBusiness" @UAC:29
