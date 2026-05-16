# SPEC1071 Component Themes Implementation Findings

Audit date: 2026-05-16

This README records the current implementation status for the scoped component theming work. The audit covered:

- `src/components/lib/Theme/IComponentTheme.cs`
- `src/components/lib/Theme/ComponentThemeService.cs`
- `src/components/lib/Theme/ThemeProviderContext.cs`
- `src/components/lib/ThemeProvider/ThemeProvider.razor`
- `src/components/lib/ThemeToggle/ThemeToggle.razor`
- `src/components/app/Program.cs`
- `src/components/app/Services/Themes/*.cs`
- `src/components/app/Showcode/Stories/Misc/ThemeProviderStory.razor`
- `src/components/test/SPEC1071.ComponentThemes/**`

## Current implementation summary

The implementation is broadly aligned with SPEC1071. Theme definitions are stateless token bags, `ComponentThemeService` owns per-provider active theme state, `ThemeProvider` renders a scoped CSS custom-property boundary, and `ThemeToggle` switches behavior depending on whether it is inside a provider.

The demo app registers five themes in order: `Light`, `Dark`, `Ocean`, `Rose`, and `Forest`. That registration order is also the provider cycle order.

## Implemented and tested

### US01.ThemeRegistry

Status: implemented.

Evidence:

- `IComponentTheme` exposes `Name` and `Tokens`.
- `ComponentThemeService.All` preserves the injected theme order.
- `Register` establishes an initial provider theme and is idempotent for existing provider IDs.
- `Set` updates provider state and raises `ThemeChanged(providerId)`.
- `Next` advances through registered themes and wraps after the last.
- `Unregister` removes provider state without removing registered themes.
- `US01.ThemeRegistry/SC01_ThemeRegistryService.cs` covers theme shape, token keys, default registration, explicit registration, idempotence, set, next, and unregister behavior.

Notes:

- Unknown theme names are stored by `Register`/`Set`, but `GetForProvider` returns null because no matching theme object exists.
- There is no explicit invalid-theme rejection or fallback event. The provider handles this by rendering no token style.

### US02.ThemeProviderBoundary

Status: implemented.

Evidence:

- `ThemeProvider` renders a root `div` with `data-theme-provider`.
- A scoped `style` element targets `[data-theme-provider="..."]`.
- Theme tokens are serialized as CSS custom properties.
- Child content is rendered inside the provider boundary.
- `ThemeProviderContext` cascades provider ID and the scoped service to children.
- `ThemeProvider` subscribes to `ThemeChanged` and updates only when the changed provider ID matches.
- `Dispose` unsubscribes and unregisters the provider.
- Additional attributes and `Class` pass through to the root div.
- `US02.ThemeProviderBoundary/SC01_ProviderBoundary.cs` covers boundary markup, style generation, cascading context, attribute pass-through, matching updates, mismatched updates, disposal, and unknown themes.

Notes:

- The style element is emitted next to the provider div, not inside it. It is still scoped by selector.
- `OnInitialized` creates the context from the initial `Id`; changing `Id` or `Theme` parameters after initialization is not handled.

### US03.ThemeToggleBehavior

Status: implemented.

Evidence:

- Outside a `ThemeProvider`, `ThemeToggle` renders sun/moon spans and invokes a configured JS function.
- Inside a `ThemeProvider`, `ThemeToggle` renders the refresh/cycle icon and calls `ComponentThemeService.Next(providerId)`.
- Custom `AriaLabel`, `CssClass`, `IconSize`, and `JsFunction` parameters are present.
- `US03.ThemeToggleBehavior/SC01_ThemeToggle.cs` covers default rendering, custom parameters, inside-provider mode, outside-provider mode, provider cycling, JS fallback, and wraparound behavior.

Notes:

- The global JS invocation tests use loose JS interop and mainly verify no exception. They do not assert the exact JS invocation payload.

### US04.ProviderIsolation

Status: implemented.

Evidence:

- Multiple providers can be registered in the same scoped service with independent theme names.
- Sibling provider updates do not update each other's rendered style.
- Nested provider state is independent because each provider uses a separate ID.
- Disposal of one provider leaves other provider entries intact.
- Shared provider IDs intentionally target the same provider state entry.
- `US04.ProviderIsolation/SC01_ProviderIsolation.cs` covers sibling isolation, nested isolation, disposal isolation, and shared-ID behavior.

Notes:

- Shared IDs are possible and behave as a single shared state entry. This is useful to document as intentional behavior because duplicate IDs also produce duplicate `data-theme-provider` selectors in markup.

### US05.DemoAppIntegration

Status: implemented.

Evidence:

- `src/components/app/Program.cs` registers five singleton `IComponentTheme` implementations and scoped `ComponentThemeService`.
- `ThemeProviderStory` exists under `Misc/Theme Provider`.
- The story exposes boundary and nested states.
- `US05.DemoAppIntegration/SC01_DemoIntegration.cs` covers service registration, scoped lifetime, story metadata, story state source, component discovery, and real theme token sanity checks.

## Remaining gaps and risks

- Parameter updates: `ThemeProvider` does not react to later `Id` or `Theme` parameter changes after `OnInitialized`.
- Token safety: token keys and values are written verbatim into CSS. Specs intentionally allow this, but consumers should treat theme implementations as trusted code.
- Unknown themes: unknown theme names silently produce no scoped token style. That behavior is tested, but there is no diagnostic event or fallback policy.
- Duplicate IDs: shared IDs are supported as shared state, but duplicate DOM selectors can surprise consumers if accidental.
- Test code style: several SPEC1071 test files contain local `using` directives. The repo standard asks for project-level `GlobalUsings.cs` instead.

## Recommended next implementation order

1. Decide whether `ThemeProvider` should support dynamic `Id` and `Theme` parameter changes.
2. Add an explicit unknown-theme policy: fallback, warning, or strict no-style behavior.
3. Add a duplicate-ID guidance note to the public story or XML docs.
4. Tighten JS interop assertions for global `ThemeToggle` clicks.
5. Move local test `using` directives into the component test project's `GlobalUsings.cs` when doing cleanup.
