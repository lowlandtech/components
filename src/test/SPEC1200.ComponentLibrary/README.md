# SPEC1200 - shadcn/ui Component Library for Blazor

## Overview

This specification covers porting the shadcn/ui React component library to Blazor. The goal is to create a comprehensive set of UI components that follow the shadcn/ui design patterns while being fully native to Blazor.

**Source**: shadcn/ui v4 components from `/lowlandtech/samples/shadcn-ui/apps/v4/registry/bases/base/ui/`

## Key Conversion Patterns

### 1. Class Variance Authority (CVA) → VariantBuilder

**React (cva)**:
```tsx
const buttonVariants = cva(
  "inline-flex items-center justify-center",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground",
        outline: "border border-input"
      },
      size: {
        default: "h-10 px-4",
        sm: "h-9 px-3"
      }
    },
    defaultVariants: {
      variant: "default",
      size: "default"
    }
  }
)
```

**Blazor (VariantBuilder)**:
```csharp
private static readonly VariantBuilder ButtonVariants = new(
    "inline-flex items-center justify-center",
    new Dictionary<string, Dictionary<string, string>> {
        ["variant"] = new() {
            ["default"] = "bg-primary text-primary-foreground",
            ["outline"] = "border border-input"
        },
        ["size"] = new() {
            ["default"] = "h-10 px-4",
            ["sm"] = "h-9 px-3"
        }
    },
    new() {
        ["variant"] = "default",
        ["size"] = "default"
    }
);

// Usage
var classes = ButtonVariants.Build(
    new() { ["variant"] = Variant, ["size"] = Size },
    Class
);
```

### 2. cn() Utility → CssClassUtils.Merge

**React**:
```tsx
className={cn("base", conditional && "extra", className)}
```

**Blazor**:
```csharp
class="@CssClassUtils.Merge("base", Conditional ? "extra" : null, Class)"
```

### 3. Portal Rendering

**React (createPortal)**:
```tsx
{open && createPortal(
  <div>{children}</div>,
  document.body
)}
```

**Blazor (Portal component)**:
```razor
@if (Open)
{
    <Portal>
        <div>@ChildContent</div>
    </Portal>
}
```

### 4. Component Props → Parameters

**React**:
```tsx
interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "default" | "outline"
  size?: "default" | "sm"
  asChild?: boolean
}

const Button = ({ variant = "default", size = "default", className, ...props }: ButtonProps) => {
  return <button className={cn(buttonVariants({ variant, size }), className)} {...props} />
}
```

**Blazor**:
```razor
@inherits ComponentBase

<button class="@_classes" @attributes="AdditionalAttributes">
    @ChildContent
</button>

@code {
    [Parameter] public string? Variant { get; set; } = "default";
    [Parameter] public string? Size { get; set; } = "default";
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] 
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string _classes => ButtonVariants.Build(
        new() { ["variant"] = Variant, ["size"] = Size },
        Class
    );
}
```

### 5. Compound Components → Cascading Parameters

**React**:
```tsx
<Dialog>
  <DialogTrigger>Open</DialogTrigger>
  <DialogContent>Content</DialogContent>
</Dialog>
```

**Blazor**:
```razor
<Dialog>
    <DialogTrigger>Open</DialogTrigger>
    <DialogContent>Content</DialogContent>
</Dialog>

@code {
    // Dialog.razor
    [CascadingParameter] private Dialog? Dialog { get; set; }
}
```

### 6. React Hooks → Blazor Lifecycle

| React Hook | Blazor Equivalent |
|------------|-------------------|
| `useState` | `private bool _state;` + `StateHasChanged()` |
| `useEffect(() => {}, [])` | `OnInitializedAsync()` |
| `useEffect(() => {})` | `OnAfterRenderAsync()` |
| `useRef` | `ElementReference` |
| `useCallback` | `EventCallback` |
| `useContext` | `CascadingParameter` |

### 7. Event Handlers

**React**:
```tsx
<button onClick={(e) => handleClick(e)}>Click</button>
```

**Blazor**:
```razor
<button @onclick="HandleClick">Click</button>

@code {
    private void HandleClick(MouseEventArgs e) { }
}
```

### 8. Conditional Rendering

**React**:
```tsx
{open && <Dialog>...</Dialog>}
{loading ? <Spinner /> : <Content />}
```

**Blazor**:
```razor
@if (Open)
{
    <Dialog>...</Dialog>
}

@if (Loading)
{
    <Spinner />
}
else
{
    <Content />
}
```

## Component Status

### US01 - Foundation Utilities (56 UACs)
- ✅ CssClassUtils.Merge - cn() function port
- ✅ CssClassUtils.MergeAttributes - Attribute dictionary merging
- ⏳ VariantBuilder - CVA port
- ⏳ ThemeService - Dark mode management
- ⏳ Portal - Overlay rendering component

### US02 - Core Components (82 UACs)
- ⏳ Button
- ⏳ Badge
- ⏳ Avatar
- ⏳ Card
- ⏳ Alert
- ⏳ Label
- ⏳ Separator
- ⏳ Skeleton

### Remaining User Stories
- US03 - Basic Form Components
- US04 - Advanced Form Components
- US05 - Overlay Components
- US06 - Navigation Components
- US07 - Layout Components
- US08 - Third-Party Integrations
- US09 - Blocks
- US10 - MudBlazor-Inspired Components

## Testing Strategy

All components are tested using:
- **bUnit** - Blazor component testing
- **xUnit** - Test framework
- **Shouldly** - Assertion library
- **Moq** - Mocking framework (for JSInterop)

Test files are located in `src/test/SPEC1200.ComponentLibrary/` alongside feature files.

## Build Process

The component library uses:
- **Tailwind CSS v4** - Utility-first CSS framework
- **NPM integration** - Automated via MSBuild
- **Isolated Node environment** - All Node files in `wwwroot/`

Build automatically runs:
1. `npm install` (if node_modules missing)
2. `npm run build` (compiles Tailwind CSS)
3. C#/Razor compilation

## Theme System

Uses CSS custom properties for theming:
- Light/dark mode support
- HSL color values
- Configurable via `wwwroot/css/variables.css`

## Key Differences from React

1. **No JSX** - Use Razor syntax instead
2. **Strongly typed** - C# type system enforces correctness
3. **Server-side capable** - Works with SSR, WASM, and Hybrid
4. **No virtual DOM** - Blazor has its own diffing algorithm
5. **JSInterop required** - For DOM manipulation outside Blazor's control

## Resources

- shadcn/ui Documentation: https://ui.shadcn.com
- Tailwind CSS v4: https://tailwindcss.com
- bUnit Documentation: https://bunit.dev
- Blazor Documentation: https://docs.microsoft.com/aspnet/core/blazor
