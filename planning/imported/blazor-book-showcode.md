---
title: "Components: adapt blazor-book as showcode app with LowlandTech components & chadcn styling"
description: Clone samples/blazor-book into src/components/app, port to .NET 10, replace presentation layer with LowlandTech components, apply chadcn/Tailwind styling, and create stories for all 50+ components.
assignee: sonnet
priority: medium
status: done
date: 2026-04-18
tags: [components, blazor-book, showcode, storybook, chadcn, tailwind]
---

# Components: blazor-book showcode app

## Problem

The current `src/components/app` is a basic WASM demo app with individual `Pages/Visual/` pages per component. It lacks a unified storybook-style browser, making it hard to discover, compare, and navigate component variants. Meanwhile `samples/blazor-book` provides exactly that UI — a sidebar-driven story browser — but it targets .NET 8, uses `BlazorSpaces` for layout, and ships its own vanilla CSS. We want to eat our own dog food: the presentation layer of the showcode app itself should use LowlandTech components with chadcn/Tailwind styling.

## Goal

A single showcode app at `src/components/app` that:

1. Provides a storybook-like browser (sidebar + main preview pane) powered by **LowlandTech components** for all chrome.
2. Registers and renders stories for every component in `src/components/lib`.
3. Uses the project's **chadcn-based Tailwind design system** (variables, themes, tokens) — not blazor-book's bundled CSS.
4. Runs on **.NET 10** (matching the rest of the solution).

## Phases

### Phase 1 — Scaffold & port blazor-book to .NET 10

| Task | Detail |
|------|--------|
| Copy core library code | Copy `samples/blazor-book/BlazorBook/` into `src/components/app/BlazorBook/` (or a subfolder). Only the core files: `BlazorBook.cs`, `CoreUtils.cs`, `Resources.cs`, story component base class. |
| Retarget to net10.0 | Remove .NET 8 package refs. Reference `Microsoft.AspNetCore.Components.Web` 10.x already in the solution. |
| Drop BlazorSpaces | Remove the `BlazorSpaces` package reference and any `LeftResizable`/`Fill` usages. These will be replaced in Phase 2. |
| Drop embedded CSS/JS | Remove `styles.css`, `spaces.css`, `spaces.js` embedded resources. Styling comes from the shared design system. |
| Verify build | `dotnet build src/components/app` compiles cleanly. |

### Phase 2 — Replace presentation layer with LowlandTech components

Replace every blazor-book chrome component with its LowlandTech equivalent:

| blazor-book element | LowlandTech replacement | Notes |
|---------------------|------------------------|-------|
| `LeftResizable` (sidebar) | `Resizable` + `ResizablePanel` | Use `Resizable` component with a collapsible left panel. |
| `<div class="sidebar">` | `Sidebar` / `SidebarContent` | Full sidebar component with header, content, footer slots. |
| `TopBar` | `NavigationMenu` or custom header | Use `NavigationMenu` or a simple flex header with `Button` + theme toggle. |
| `ComponentList` (tree) | `Accordion` + `Collapsible` | Render story groups as accordion items, individual stories as links. |
| `MainPane` (iframe) | Direct Razor rendering | Render the selected `StoryComponent` inline via `DynamicComponent` instead of an iframe. Eliminates the iframe isolation overhead. |
| Theme toggle | Existing `ThemeToggle` component | Already in `src/components/app/Components/`. |

### Phase 3 — Apply chadcn / Tailwind styling

| Task | Detail |
|------|--------|
| Remove blazor-book CSS | Delete all blazor-book `.less` / `.css` files. |
| Use shared `variables.css` | Import from `src/components/lib/wwwroot/css/variables.css` (design tokens, light/dark theme). |
| Tailwind config | Ensure the app's Tailwind build scans both `src/components/app/**/*.razor` and `src/components/lib/**/*.razor`. |
| Layout classes | Sidebar: `border-r`, `bg-sidebar`, `text-sidebar-foreground`. Main pane: `bg-background`, `text-foreground`. Use existing CSS variables. |
| Dark mode | Wire up `ThemeService` → `.dark` class on `<html>`. Already implemented; just verify it works with the new layout. |

### Phase 4 — Create stories for all components

One story file per component (or group), inheriting from `StoryComponent`:

```razor
@inherits BlazorBook.StoryComponent
@attribute [DisplayName("Form/Button")]

<div class="space-y-4">
    <h3>Variants</h3>
    <div class="flex gap-2">
        <Button Variant="default">Default</Button>
        <Button Variant="destructive">Destructive</Button>
        <Button Variant="outline">Outline</Button>
        <Button Variant="secondary">Secondary</Button>
        <Button Variant="ghost">Ghost</Button>
        <Button Variant="link">Link</Button>
    </div>

    <h3>Sizes</h3>
    <div class="flex gap-2 items-center">
        <Button Size="sm">Small</Button>
        <Button Size="default">Default</Button>
        <Button Size="lg">Large</Button>
    </div>
</div>
```

**Grouping convention** (via `DisplayName` path):

| Group | Components |
|-------|-----------|
| `Layout/` | AspectRatio, Card, Separator, Skeleton, Resizable, Empty |
| `Navigation/` | Accordion, Breadcrumb, Menubar, NavigationMenu, Pagination, Sidebar, Tabs, Command |
| `Form/` | Button, ButtonGroup, Checkbox, Input, InputGroup, InputOTP, Label, NativeSelect, RadioGroup, SelectInput, Slider, Switch, Textarea, Toggle, ToggleGroup |
| `Data/` | Calendar, Chart, Table, Carousel |
| `Overlay/` | AlertDialog, Dialog, Drawer, Dropdown, HoverCard, Popover, Sheet, Tooltip |
| `Feedback/` | Alert, Badge, Progress, Sonner, Spinner |
| `Misc/` | Avatar, CodeEditor, Collapsible, Combobox, ContextMenu, Icon, Kbd, ScrollArea |

Each story should demonstrate:
- All variants / sizes where applicable
- Light and dark rendering (inherits from theme)
- Interactive state (e.g., accordion expand, dialog open/close)
- Composition examples (e.g., Card with CardHeader + CardContent + CardFooter)

### Phase 5 — Clean up and integrate

| Task | Detail |
|------|--------|
| Remove old `Pages/Visual/` | Delete the per-component preview pages that are now replaced by stories. |
| Update `NavMenu` / routing | Point the app's default route to the storybook UI. Keep `/` redirecting to `/blazorbook` or embed inline. |
| Update `_Imports.razor` | Add `@using BlazorBook` (or equivalent namespace) for story base class. |
| Verify solution build | `dotnet build foundry.slnx` passes. |
| Update `docs/components/readme.md` | Note the new showcode app and how to add stories. |

## Out of scope

- **Visual regression testing** — existing Playwright pipeline remains unchanged; it can be pointed at stories later.
- **WASM vs Server hosting** — keep the current WASM hosting model.
- **Publishing blazor-book as a NuGet** — the code is inlined, not packaged.
- **Story controls / knobs** — interactive parameter editors (like Storybook Controls) are a follow-up.

## Acceptance criteria

- [ ] `src/components/app` builds on .NET 10 with no blazor-book .NET 8 dependencies.
- [ ] Sidebar, layout, and navigation use only LowlandTech components (no BlazorSpaces).
- [ ] All styling uses the chadcn/Tailwind design system (variables.css, utility classes).
- [ ] Stories exist for at least the 20 components currently at 👍 Accepted status.
- [ ] Stories render correctly in both light and dark themes.
- [ ] Selected story renders inline (no iframe).
- [ ] Old `Pages/Visual/` preview pages are removed.
- [ ] `dotnet build foundry.slnx` succeeds.

---

## Navigation

[[components/issues/_index|Back to Index]]