# LowlandTech Blazor Components

A comprehensive Blazor component library inspired by shadcn/ui, built with .NET 10.0 and Tailwind CSS.

## Project Status

- **Build Status**: ✅ 0 Warnings, 0 Errors
- **Test Coverage**: ✅ 257 Tests, 100% Passing
- **Components**: 22 / 57 Complete (39%)

## Component Library

### Core Components

| Component | Status | Tests | Description |
|-----------|--------|-------|-------------|
| Badge | ✅ Complete | ✅ Passing | Status indicators and labels |
| Button | ✅ Complete | ✅ Passing | Interactive button component |
| Checkbox | ✅ Complete | ✅ Passing | Checkbox input with label |
| Input | ✅ Complete | ✅ Passing | Text input field |
| Label | 📋 Planned | - | Form field labels |
| Textarea | ✅ Complete | ✅ Passing | Multi-line text input |
| Separator | ✅ Complete | ✅ Passing | Visual divider (horizontal/vertical) |

### Form Components

| Component | Status | Tests | Description |
|-----------|--------|-------|-------------|
| Combobox | 📋 Planned | - | Autocomplete/searchable select |
| Input-OTP | 📋 Planned | - | One-time password input |
| Native-Select | 📋 Planned | - | Native HTML select dropdown |
| Radio-Group | 📋 Planned | - | Radio button groups |
| Select | 📋 Planned | - | Custom dropdown select menu |
| Slider | 📋 Planned | - | Range slider input |
| Switch | 📋 Planned | - | Toggle switch |

### Overlay Components

| Component | Status | Tests | Description |
|-----------|--------|-------|-------------|
| AlertDialog | ✅ Complete | ✅ Passing | Critical confirmation dialogs |
| Dialog | ✅ Complete | ✅ Passing | Modal dialog component |
| DropdownMenu | ✅ Complete | ✅ Passing | Contextual dropdown menus |
| HoverCard | ✅ Complete | ✅ Passing | Popover on hover |
| Popover | ✅ Complete | ✅ Passing | Positioned popover overlay |
| Sheet | ✅ Complete | ✅ Passing | Side panel/drawer component |
| Tooltip | ✅ Complete | ✅ Passing | Hover tooltips |
| Context-Menu | 📋 Planned | - | Right-click context menu |
| Drawer | 📋 Planned | - | Bottom drawer (mobile-friendly) |
| Menubar | 📋 Planned | - | Application menu bar |

### Navigation Components

| Component | Status | Tests | Description |
|-----------|--------|-------|-------------|
| Accordion | ✅ Complete | ✅ Passing | Collapsible content sections |
| Tabs | ✅ Complete | ✅ Passing | Tabbed navigation |
| Breadcrumb | 📋 Planned | - | Breadcrumb navigation |
| Navigation-Menu | 📋 Planned | - | Complex navigation menus |
| Pagination | 📋 Planned | - | Page navigation |
| Sidebar | 📋 Planned | - | Application sidebar |

### Layout Components

| Component | Status | Tests | Description |
|-----------|--------|-------|-------------|
| Card | 📋 Planned | - | Content card container |
| Stack | ✅ Complete | ✅ Passing | Flex-based stacking layout |
| Table | ✅ Complete | ✅ Passing | Data table component |
| Aspect-Ratio | 📋 Planned | - | Maintain aspect ratios |
| Collapsible | 📋 Planned | - | Collapsible content |
| Resizable | 📋 Planned | - | Resizable panels |
| Scroll-Area | 📋 Planned | - | Custom scrollable area |

### Feedback Components

| Component | Status | Tests | Description |
|-----------|--------|-------|-------------|
| Alert | 📋 Planned | - | Alert/notification messages |
| Progress | 📋 Planned | - | Progress bar indicator |
| Skeleton | ✅ Complete | ✅ Passing | Loading skeleton placeholders |
| Spinner | 📋 Planned | - | Loading spinner |
| Sonner | 📋 Planned | - | Toast notifications |

### Display Components

| Component | Status | Tests | Description |
|-----------|--------|-------|-------------|
| Avatar | 📋 Planned | - | User avatar component |
| Timeline | ✅ Complete | ✅ Passing | Vertical timeline component |
| Stepper | ✅ Complete | ✅ Passing | Multi-step workflow indicator |
| Carousel | 📋 Planned | - | Image/content carousel |
| Chart | 📋 Planned | - | Data visualization charts |

### Utility Components

| Component | Status | Tests | Description |
|-----------|--------|-------|-------------|
| Icons | ✅ Complete | ✅ Passing | Icon components |
| Portal | ✅ Complete | ✅ Passing | DOM portal for rendering |
| Button-Group | 📋 Planned | - | Grouped buttons |
| Empty | 📋 Planned | - | Empty state component |
| Field | 📋 Planned | - | Form field wrapper |
| Input-Group | 📋 Planned | - | Grouped inputs |
| Item | 📋 Planned | - | List item component |
| Kbd | 📋 Planned | - | Keyboard shortcut display |
| Toggle | 📋 Planned | - | Toggle button |
| Toggle-Group | 📋 Planned | - | Toggle button group |
| Calendar | 📋 Planned | - | Date picker calendar |
| Command | 📋 Planned | - | Command palette (Cmd+K) |

## Development Roadmap

### ✅ Phase 0: Foundation (Complete)
- Core component library structure
- Test infrastructure (xUnit + bUnit)
- Tailwind CSS integration
- 22 foundational components
- Zero build warnings

### 🚀 Phase 1: Essential Form Components (Planned)
1. Label - Form field labels
2. Select - Custom dropdown select
3. Radio-Group - Radio button groups
4. Switch - Toggle switch control

### 📋 Phase 2: Core UI Components (Planned)
1. Card - Content card container
2. Alert - Alert/notification messages
3. Avatar - User avatar component
4. Progress - Progress bar indicator

### 📋 Phase 3: Navigation Components (Planned)
1. Breadcrumb - Breadcrumb navigation
2. Pagination - Page navigation
3. Navigation-Menu - Complex navigation
4. Sidebar - Application sidebar

## Technology Stack

- **.NET 10.0** - Latest .NET framework
- **Blazor** - Component model
- **Tailwind CSS v4.1** - Utility-first CSS
- **xUnit** - Testing framework
- **bUnit 2.5.3** - Blazor component testing
- **Shouldly** - Assertion library

## Getting Started

### Prerequisites
- .NET 10.0 SDK
- Node.js (for Tailwind CSS)

### Build

```bash
cd src/lib
dotnet build
```

### Run Tests

```bash
cd src/test
dotnet test
```

## Project Structure

```
/root/components/
├── src/
│   ├── lib/                          # Component library
│   │   ├── Components/               # All components
│   │   ├── Utilities/                # Shared utilities
│   │   └── wwwroot/                  # Static assets & Tailwind
│   └── test/                         # Test project
│       └── SPEC1200.ComponentLibrary/ # Test specifications
├── samples/
│   └── chadcn/                       # shadcn/ui reference
└── README.md
```

## Contributing

Components follow the BDD testing pattern with Given/When/Then scenarios. Each component includes:
- Razor component files (.razor)
- Code-behind files (.razor.cs)
- Comprehensive test coverage
- XML documentation

## License

[Your License Here]

## Acknowledgments

- Inspired by [shadcn/ui](https://ui.shadcn.com/)
- Built with [Radix UI](https://www.radix-ui.com/) patterns
- Styled with [Tailwind CSS](https://tailwindcss.com/)
