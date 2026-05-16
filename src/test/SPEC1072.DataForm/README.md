# SPEC1072 DataForm Implementation Findings

Audit date: 2026-05-16

This README records the current implementation status for `LowlandTech.Components.DataForm.DataForm`. The audit covered:

- `src/components/lib/DataForm/DataForm.razor`
- `src/components/lib/DataForm/DataForm.razor.cs`
- `src/components/lib/DataForm/DataFormField.cs`
- `src/components/app/Showcode/Stories/Form/DataFormStory.razor`
- `src/components/test/SPEC1072.DataForm/**`

## Current implementation summary

DataForm is implemented as a reflection-driven generated form over a supplied model. It creates or accepts an `EditContext`, discovers public writable properties, maps property types to default inputs, writes input values back to the model, and can render labels, fields, validation messages, and actions through templates.

The current implementation covers a practical first slice of US01 and US06. It does not yet implement the requested FluentValidatorAttribute, ValidatorDiscoveryService, NCalc condition evaluation, async validation lifecycle, or full DataAnnotations integration.

## Implemented and tested

### US01.ModelBinding

Status: mostly implemented.

Evidence:

- `DataForm` creates an `EditContext` when only `Model` is supplied.
- It cascades the `EditContext` to child content.
- Public readable and writable model properties become `DataFormField` entries.
- `IncludeProperties` and `ExcludeProperties` shape the generated field set.
- `DisplayAttribute.Name` and `DisplayAttribute.Order` are used for labels and ordering.
- Default input mapping is implemented:
  - string and unknown types -> text input
  - numeric types -> number input
  - bool -> checkbox input
  - enum -> `Select`
  - `DateTime` and `DateOnly` -> date input
- Field input writes values back to the supplied model.
- `NotifyFieldChanged` is called after field updates.
- New model instances rebuild the `EditContext` and rediscover fields.
- Read-only mode renders display values and suppresses editable inputs.
- `US01.ModelBinding/SC01_DataFormModelBinding.cs` covers these behaviors.

Notes:

- `DataFormField.ReadOnly` is populated from the component-level `ReadOnly` parameter only. There is no per-property read-only metadata yet.
- `DataFormField.Required` is discovered from `[Required]`, but the generated markup does not currently expose required markers or `aria-required`.
- Clearing a non-nullable numeric field writes the default numeric value (`0`) instead of keeping invalid text or producing a conversion validation message. This is defensive behavior to prevent `ArgumentException` crashes from type mismatches.

### US06.RenderingExtensibility

Status: partially implemented.

Evidence:

- `Layout` supports `vertical`, `grid`, and `horizontal` class branches.
- Root form renders `data-slot="data-form"` and `data-layout`.
- `FieldTemplate`, `LabelTemplate`, `ValidationTemplate`, and `ActionsTemplate` parameters exist.
- Default actions render Submit and Reset buttons when `ShowDefaultActions` is true.
- Validation summary and field validation regions render with alert semantics when messages exist.
- `DataFormStory.razor` demonstrates default, grid, read-only, validation, and template states.

Missing or partial:

- There are no scenario tests yet for layout, templates, validation message accessibility, validation summary links, actions template context, or focus behavior.
- `FieldTemplate` receives only `DataFormField`, not a richer context with current value, setter, messages, dirty state, or focus target.
- `ActionsTemplate` receives the `DataForm` component instance, not a dedicated actions context with submit/reset/dirty/validating state.
- There is no field focus API.
- Grid columns are fixed at `sm:grid-cols-2`; the spec's configurable column count is not implemented.

## Spec-only areas

### US02.DataAnnotationsValidation

Status: mostly missing.

`HandleSubmit` calls `_editContext.Validate()`, but `DataForm` does not add a `DataAnnotationsValidator`. Without a validator registered inside the cascaded `EditContext`, DataAnnotations attributes such as `[Required]`, `[EmailAddress]`, `[Range]`, and `[StringLength]` will not necessarily produce messages.

Current implementation:

- Calls `EditContext.Validate()` on submit when `ValidateOnSubmit` is true.
- Copies `EditContext.GetValidationMessages()` into `_validationSummary`.
- Invokes `OnValidSubmit` or `OnInvalidSubmit` based on validation result.

Missing / partially implemented:

- ✅ `<DataAnnotationsValidator />` is now rendered inside the form so `EditContext.Validate()` can execute DataAnnotations rules.
- Field-level validation on input or blur is not yet wired.
- Required, range, length, and email tests are not yet written.
- Field-level message population from DataAnnotations now works at submit time via the validator.

### US03.FluentValidatorDiscovery

Status: missing.

There is no implementation for:

- `FluentValidatorAttribute`.
- String or enum validator keys.
- `ValidatorDiscoveryService`.
- Property-level validators.
- Model-level validators.
- FluentValidation package usage in the component library.

### US04.ConditionalValidation

Status: missing.

There is no implementation for:

- `Condition` on validator attributes.
- NCalc expression evaluation.
- Operand binding from model properties.
- Conditional validator dependency tracking.
- Safe error behavior for malformed conditions.

### US05.ValidationLifecycle

Status: mostly missing.

There is only submit-time validation through `EditContext.Validate()`.

Missing:

- Validate-on-input.
- Validate-on-blur.
- Async validator tracking.
- Pending validator state.
- Submission waiting for async validation.
- Merged messages from DataAnnotations and discovered validators.
- Validation lifecycle events.

## Fixed since audit

- ✅ Local `using` directives removed from `DataForm.razor.cs` and `Button.razor.cs`; `System.Globalization` and `Microsoft.AspNetCore.Components.Web` added to `GlobalUsings.cs`.
- ✅ `OnParametersSet` now detects `EditContext` parameter changes independently of `Model` changes (`_currentEditContextParam` tracking).
- ✅ `ResetModel` now restores original property values via `SnapshotValues()` / `_originalValues` dictionary.
- ✅ `SetValue` wrapped in `try/catch`; type-compatibility guard with `Convert.ChangeType` fallback prevents `ArgumentException` crashes.
- ✅ `ConvertValue` uses `CultureInfo.InvariantCulture` for all parsing and returns `null` on failure instead of raw strings.
- ✅ `<form @onsubmit:preventDefault>` prevents accidental browser form submission.
- ✅ `GetDisplayValue` formats `DateTime` / `DateOnly` as `yyyy-MM-dd` for HTML `type="date"` inputs.
- ✅ `<DataAnnotationsValidator />` added inside the cascaded `EditContext`.

## Remaining implementation risks and follow-up notes

- `_fieldMessages` is maintained but not populated; rendered field messages come directly from `_editContext.GetValidationMessages(...)`.
- `OnModelChanged.InvokeAsync()` is fire-and-forget inside `SetValue`; async callback failures are suppressed by `try/catch` but not surfaced to the caller.
- The template story's `FieldTemplate` uses `Input` with a value but does not show a clear write-back contract to DataForm.

## Recommended next implementation order

1. Add internal DataAnnotations validation support and tests for US02.
2. Add a richer `DataFormFieldContext` and `DataFormActionsContext` for templates.
3. Implement `FluentValidatorAttribute` plus `ValidatorDiscoveryService` for US03.
4. Add NCalc-backed condition evaluation for US04.
5. Add validate-on-input, validate-on-blur, async pending state, and lifecycle events for US05.
6. Add focused tests for US06 layout, templates, validation markup, summary behavior, and custom actions.
7. Move local `using` directives into `GlobalUsings.cs` during cleanup.
