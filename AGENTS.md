# Agent Guidelines - LowlandTech Code

This file contains essential information for AI coding agents working in this repository. It covers build commands, testing, code style, and architectural patterns.

---

## Project Overview

**.NET 10 Multi-Project Solution** with Blazor Server UI, Core library, API, CLI, and component library using shadcn/ui-inspired design system.

**Tech Stack:** C# 13, .NET 10, Blazor Server, Tailwind CSS v4, xUnit, Shouldly, bUnit, PostgreSQL

**Solution Structure:**
- `src/core/` - Core domain models and services (LowlandTech.Core)
- `src/api/` - ASP.NET Core Web API (LowlandTech.Api)
- `src/app/` - Blazor Server application (LowlandTech.App)
- `src/cli/` - Command-line interface (LowlandTech.Cli)
- `src/components/` - Blazor component library with Tailwind CSS (LowlandTech.Components)
- `src/test/` - xUnit test suite with BDD-style organization (LowlandTech.Tests)

---

## Build / Lint / Test Commands

### Building

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/api/LowlandTech.Api.csproj
dotnet build src/app/LowlandTech.App.csproj

# Build in Release mode
dotnet build -c Release

# Restore dependencies
dotnet restore
```

### Running Applications

```bash
# Run API server (default: http://localhost:5000)
cd src/api
dotnet run

# Run Blazor app (default: http://localhost:5000)
cd src/app
dotnet run --urls http://localhost:5000

# Run CLI tool
cd src/cli
dotnet run -- [arguments]
```

### Testing

**Run all tests:**
```bash
dotnet test
```

**Run specific test project:**
```bash
dotnet test src/test/LowlandTech.Tests.csproj
```

**Filter by specification/user story/scenario/UAC (most important for targeted testing):**
```bash
# Run all tests for a specification
dotnet test --filter "SPEC=0001"

# Run all tests for a user story
dotnet test --filter "SPEC=0005&US=01"

# Run a specific scenario (all its UACs)
dotnet test --filter "SPEC=0005&US=01&SC=01"

# Run a specific acceptance criterion
dotnet test --filter "SPEC=0005&US=01&SC=01&UAC=01"
```

**Run single test by name:**
```bash
dotnet test --filter "FullyQualifiedName~UAC01_AscendingIdsShouldBeOrdered"
dotnet test --filter "DisplayName~organization name should be"
```

**Run with verbosity:**
```bash
dotnet test -v detailed
dotnet test --logger "console;verbosity=detailed"
```

### Tailwind CSS

```bash
# Install npm dependencies (runs automatically on build)
cd src/components/wwwroot
npm install

# Build CSS manually
npm run build:css

# Watch mode for development
npm run watch:css
```

---

## Code Style Guidelines

### Naming Conventions

**C# Interfaces:** PascalCase with `I` prefix
```csharp
ISessionService, IMessageService, IStorage, IEventBus
```

**C# Classes:** PascalCase
```csharp
SessionService, MessageService, JsonFileStorage, InMemoryEventBus
```

**C# Private Fields:** camelCase with `_` prefix
```csharp
private readonly IStorage _storage;
private readonly IEventBus _eventBus;
private bool _disposed;
```

**C# Methods:** PascalCase, async methods end with `Async`
```csharp
Task<SessionInfo?> GetSessionAsync(string sessionId, CancellationToken ct = default);
Task InitializeAsync(CancellationToken cancellationToken = default);
```

**C# Properties:** PascalCase
```csharp
public string SessionId { get; set; }
public DateTime CreatedAt { get; init; }
```

**C# Local Variables:** camelCase
```csharp
var sessionId = Identifier.Ascending(EntityPrefix.Session);
var messages = await GetMessagesAsync(sessionId);
```

### Type Usage

**Nullable Reference Types: MANDATORY (enabled globally)**
```csharp
✅ CORRECT
public string? Name { get; set; }
SessionInfo? session = await GetSessionAsync(id);
event EventHandler<MessageEventArgs>? MessageAdded;

❌ INCORRECT - Will cause compiler warnings
public string Name { get; set; }
SessionInfo session = await GetSessionAsync(id);
```

**Prefer `var` for obvious types:**
```csharp
var service = new SessionService(storage, eventBus);
var sessions = new List<SessionInfo>();
```

**Use explicit types for clarity:**
```csharp
ISessionService service = serviceProvider.GetRequiredService<ISessionService>();
CancellationToken cancellationToken = default;
```

### Async/Await Patterns (CRITICAL)

**ALL methods in execution path MUST be async Task:**
```csharp
✅ CORRECT
[Fact]
public async Task UAC01_SessionShouldBeCreated() { }

protected override async Task OnInitializedAsync() { }
private async Task HandleClickAsync() { }

❌ FORBIDDEN - Causes test hangs and runtime issues
[Fact]
public void MyTest() { }                    // Sync test
private void Setup() { }                    // Sync helper
private async void OnStateChanged() { }     // async void
```

**Default CancellationToken pattern:**
```csharp
Task<SessionInfo?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);
```

**Never block on async calls:**
```csharp
✅ CORRECT
await service.InitializeAsync(cancellationToken);

❌ INCORRECT - Causes deadlocks
service.InitializeAsync(cancellationToken).Wait();
service.InitializeAsync(cancellationToken).GetAwaiter().GetResult();
```

### Error Handling

**Return null for not-found scenarios:**
```csharp
public async Task<SessionInfo?> GetSessionAsync(string sessionId, CancellationToken ct = default)
{
    var path = GetPath(sessionId);
    return await _storage.GetAsync<SessionInfo>(path, ct);
}
```

**Throw specific exceptions for errors:**
```csharp
throw new StorageNotFoundException($"Session {sessionId} not found");
throw new InvalidOperationException("Permission request already answered");
```

**Use ArgumentNullException.ThrowIfNull:**
```csharp
public SessionService(IStorage storage, IEventBus eventBus)
{
    ArgumentNullException.ThrowIfNull(storage);
    ArgumentNullException.ThrowIfNull(eventBus);
    _storage = storage;
    _eventBus = eventBus;
}
```

### Imports and Using Statements

**Using statements at top of file, grouped by namespace:**
```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LowlandTech.Core.Events;
using LowlandTech.Core.Storage;
using Shouldly;
using Xunit;
```

**Global usings in test project:**
See `src/test/GlobalUsings.cs` for test-wide imports (Xunit, Shouldly, common namespaces).

### Control Flow

**Use early returns instead of else:**
```csharp
✅ CORRECT
if (session == null)
    return null;

return session.Messages;

❌ INCORRECT
if (session != null)
{
    return session.Messages;
}
else
{
    return null;
}
```

**Prefer ternary for simple assignments:**
```csharp
var status = isEnabled ? "enabled" : "disabled";
```

---

## Testing Standards - Specification by Example

### Philosophy

Tests are **executable specifications** organized in a 4-level hierarchy:
```
SPEC0005.US01.SC01.UAC01
|        |    |    └─ User Acceptance Criterion
|        |    └────── Scenario
|        └─────────── User Story
└──────────────────── Specification
```

Every test traces back to business requirements and serves as living documentation.

### Folder Structure

```
src/test/
├── Scenario.cs                          # Base class for all tests
├── Spec.cs                              # Trait constants (SPEC, US, SC, UAC)
├── GlobalUsings.cs                      # Global imports
│
├── SPEC0001.Identity/
│   └── US01.IdGeneration/
│       ├── SC01_AscendingIds.cs
│       ├── SC02_DescendingIds.cs
│       └── SC03_PrefixValidation.cs
│
├── SPEC0005.SessionService/
│   ├── SessionServiceFixture.cs         # Shared test data/setup
│   ├── US01.SessionCrud/
│   │   ├── SC01_CreateRootSession.cs
│   │   └── SC02_CreateChildSession.cs
│   └── US02.SessionFork/
│       └── SC01_ForkCopiesMessages.cs
```

**Naming conventions:**
- Spec folders: `SPECXXXX.ComponentName/`
- User Story folders: `USXX.StoryName/`
- Scenario classes: `SCXX_ScenarioName.cs`

### Test Class Pattern

```csharp
using LowlandTech.Core.Session;

namespace LowlandTech.Tests.SPEC0005.SessionService.US01.SessionCrud;

[Trait(Spec.SPEC, "0005")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
public class SC01_CreateRootSession : Scenario
{
    private SessionInfo? _createdSession;
    private ISessionService _service = null!;

    protected override Task GivenAsync()
    {
        // Setup - corresponds to "Given" in BDD
        _service = new SessionService(storage, eventBus);
        return Task.CompletedTask;
    }

    protected override async Task WhenAsync()
    {
        // Action - corresponds to "When" in BDD
        _createdSession = await _service.CreateSessionAsync(null);
    }

    [Fact(DisplayName = "Then the session should have a valid ID")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_SessionShouldHaveValidId()
    {
        await ArrangeAndActAsync();
        _createdSession.ShouldNotBeNull();
        _createdSession.SessionId.ShouldNotBeNullOrEmpty();
    }

    [Fact(DisplayName = "And the session should have no parent")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_SessionShouldHaveNoParent()
    {
        await ArrangeAndActAsync();
        _createdSession!.ParentSessionId.ShouldBeNull();
    }
}
```

**Mandatory rules:**
- **Class-level traits:** `SPEC`, `US`, `SC` (once per scenario class)
- **Method-level traits:** `UAC` (per test method)
- **DisplayName:** Matches the BDD statement (Then/And)
- **Method name:** Always `UACXX_DescriptiveName`
- **One assertion per test:** Each Then/And = one [Fact]
- **All async Task:** Every test method MUST be `async Task`
- **Shouldly assertions:** Use `ShouldBe()`, `ShouldNotBeNull()`, etc.

### Shouldly Assertions

```csharp
// Equality
result.ShouldBe(expected);
result.ShouldNotBe(other);

// Nullability
obj.ShouldBeNull();
obj.ShouldNotBeNull();

// Strings
text.ShouldBeNullOrEmpty();
text.ShouldNotBeNullOrEmpty();
text.ShouldContain("substring");
text.ShouldStartWith("prefix");

// Collections
list.ShouldBeEmpty();
list.ShouldNotBeEmpty();
list.ShouldContain(item);
list.Count.ShouldBe(5);

// Numbers
count.ShouldBeGreaterThan(0);
count.ShouldBeLessThan(100);
value.ShouldBeInRange(1, 10);

// Booleans
flag.ShouldBeTrue();
flag.ShouldBeFalse();

// Exceptions
Should.Throw<StorageNotFoundException>(() => service.GetNonexistent());
await Should.ThrowAsync<ArgumentNullException>(async () => await service.CreateAsync(null));
```

---

## Blazor & UI Guidelines

### Component Structure

**Basic component pattern:**
```razor
@page "/sessions"
@inject ISessionService SessionService

<div class="space-y-6">
    <h1 class="text-3xl font-bold">Sessions</h1>
    
    @if (_loading)
    {
        <p>Loading...</p>
    }
    else
    {
        <ul>
            @foreach (var session in _sessions)
            {
                <li>@session.SessionId</li>
            }
        </ul>
    }
</div>

@code {
    private List<SessionInfo> _sessions = [];
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadSessionsAsync();
    }

    private async Task LoadSessionsAsync()
    {
        _loading = true;
        try
        {
            _sessions = await SessionService.ListSessionsAsync();
        }
        finally
        {
            _loading = false;
        }
    }
}
```

### Tailwind CSS & Styling

**Dark theme only** - use semantic CSS variables from shadcn/ui:
```razor
✅ CORRECT
<div class="bg-background text-foreground">
<button class="bg-primary text-primary-foreground">

❌ INCORRECT
<div class="bg-white text-black">
<button class="bg-purple-600 text-white">
```

**Component classes (see .cursorrules for full details):**
- Buttons: `.btn .btn-default`, `.btn .btn-destructive`
- Cards: `.card`, `.card-header`, `.card-content`
- Inputs: `.input`, `.label`

---

## Common Patterns

### Dependency Injection

**Service registration in Program.cs:**
```csharp
builder.Services.AddSingleton<IStorage, JsonFileStorage>();
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
builder.Services.AddScoped<ISessionService, SessionService>();
```

**Constructor injection:**
```csharp
public class SessionService : ISessionService
{
    private readonly IStorage _storage;
    private readonly IEventBus _eventBus;

    public SessionService(IStorage storage, IEventBus eventBus)
    {
        ArgumentNullException.ThrowIfNull(storage);
        ArgumentNullException.ThrowIfNull(eventBus);
        _storage = storage;
        _eventBus = eventBus;
    }
}
```

### Event-Driven Architecture

**Publishing events:**
```csharp
await _eventBus.PublishAsync(new MessageAdded
{
    SessionId = sessionId,
    MessageId = messageId,
    Timestamp = DateTime.UtcNow
}, cancellationToken);
```

**Subscribing to events:**
```csharp
_eventBus.Subscribe<MessageAdded>(OnMessageAddedAsync);

private async Task OnMessageAddedAsync(MessageAdded evt)
{
    await UpdateUIAsync();
}
```

### Storage Pattern

**Reading:**
```csharp
var session = await _storage.GetAsync<SessionInfo>(path, cancellationToken);
```

**Writing:**
```csharp
await _storage.PutAsync(path, session, cancellationToken);
```

**Listing:**
```csharp
var sessions = await _storage.ListAsync<SessionInfo>("sessions/", cancellationToken);
```

---

## Key Documentation

- **Cursor Rules:** `.cursorrules` (877 lines - comprehensive style guide)
- **Chat Protocol:** `.chat/README.md` (file-based chat conventions)
- **Test Results:** `src/test/TEST_RESULTS.md`
- **Missing Specs:** `MISSING_SPECS.md`
- **API Quick Start:** `OPENCODE_API_QUICK_START.md`

---

## Important Notes

1. **Nullable everything** - All reference types must have `?` annotations
2. **All async, all the time** - Tests and async methods MUST be `async Task`
3. **One assertion per test** - Every Then/And = one [Fact] with UAC trait
4. **Filter tests with traits** - Use `--filter "SPEC=XXXX&US=XX"` for targeted testing
5. **Shouldly for assertions** - Use `ShouldBe()`, not FluentAssertions
6. **Dark theme only** - Use CSS variables, never hardcode colors
7. **Early returns** - Avoid else statements
8. **ArgumentNullException.ThrowIfNull** - Use for null checks in constructors
9. **CancellationToken default** - Always `CancellationToken ct = default`
10. **Follow .cursorrules** - Detailed UI and testing guidelines are there
11. **Commit before done** - Implementers must commit completed issue work before moving it to `done/` and record the commit hash in the issue summary
