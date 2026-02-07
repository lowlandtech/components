## Agent Chat via Files

This folder hosts an append-only, file-based "agent chat" system. Each message is a file named by a global tick and the agent name, like `000123@codex.txt`. Ticks are strictly increasing integers and define the canonical message order.

Agents should check their chat queue before exiting their loop and should treat a new chat message as equivalent to a prompt from the user.

## Repo Rules (From `.cursorrules`)

These rules govern work in this repo. If anything here conflicts with other docs, defer to `.cursorrules`.

### Must-Follow Highlights

- Dark theme only; do not add light theme styles.
- Use component classes from `input.css` (no re-creating with raw Tailwind utilities).
- Use semantic colors (CSS variables), never hardcode colors.
- All async, all the time for tests and event handlers (`async Task` only).
- Nullable reference types are mandatory.
- bUnit for component tests; Shouldly for assertions.
- Follow Agent Loop protocol: check `issues/SPECXXXX/`, update `todos.md` and `progress.md`.
- Memory handoff: before context compaction, update spec files (context, todos, progress).
- Commit after task completion with semantic message and issue reference (e.g., `feat(component): add X (#33)`).

### Canonical References

- `.cursorrules` (primary)
- `docs/AGENT_LOOP.md`
- `AGENTS.md`

### 1) Files and Naming

- Message files live in `.chat/`
- Name format: `{tick}@{agent}.txt`
- `tick` is a zero-padded integer width 6 (e.g., `000001`, `000002`)
- `agent` is a lowercase id (e.g., `codex`, `planner`, `reviewer`)

Example:

```
000001@codex.txt
000002@planner.txt
000003@codex.txt
```

### 2) Message Format

Each message file is plain text. Recommended headers:

```
tick: 000123
agent: codex
role: assistant | user | system
time: 2026-02-06T15:04:05Z
in_reply_to: 000122@planner.txt

<message body>
```

Headers are optional but strongly encouraged to keep tooling simple and deterministic.

### 3) Coordination Rules

- **Single source of truth:** Tick order defines conversation order.
- **No edits:** Messages are immutable once written. Create a new file for corrections.
- **No gaps:** Ticks should be strictly sequential.
- **One writer per tick:** If multiple agents are writing, they must coordinate to avoid conflicts.

### 4) Allocation Protocol (Simple)

1. Read the current max tick in `.chat/`.
2. Next tick = max + 1.
3. Write `NEXT_TICK` in a lock file, then write the message file.
4. Remove lock.

Lock file format (optional but recommended): `.chat/.lock`

```
tick: 000124
agent: codex
time: 2026-02-06T15:04:05Z
```

If `.lock` exists and is fresh (e.g., < 2 minutes), wait. If stale, claim it and continue.

### 5) Minimal Index (Optional)

You can keep an index file for quick lookups:

File: `.chat/index.csv`

```
tick,agent,role,time,filename
000123,codex,assistant,2026-02-06T15:04:05Z,000123@codex.txt
```

This is optional and can be generated from filenames.

### 6) Example Workflow (Manual)

1. List files: sort by name.
2. Determine next tick.
3. Create new message file.
4. Append to index (optional).

### 7) Notes

- Use ASCII for headers to keep parsing trivial.
- Keep message content as-is; do not auto-wrap.
- If you need threading, rely on `in_reply_to`.

## Plan Board (File-Based Kanban)

This is an optional, file-based planning board. Moving a file between columns is the state transition.

### 1) Structure

- `.chat/board/backlog/`
- `.chat/board/todo/`
- `.chat/board/doing/`
- `.chat/board/done/`

### 2) Issue Files

Each issue is a standalone file, e.g. `ISSUE-000123.md`.

Recommended header:

```
id: ISSUE-000123
title: Short title
owner: codex
created: 2026-02-06T15:04:05Z
status: backlog | todo | doing | done
related_messages: 000120@user.txt,000121@codex.txt

<details>
```

### 3) Rules

- Only one column at a time (single source of truth).
- Move file to update status; keep file contents immutable except for `status` and notes.
- Keep ids strictly increasing to avoid collisions.

### 4) Minimal Workflow

1. Create issue in `backlog/`.
2. Move to `todo/` when scoped.
3. Move to `doing/` when active.
4. Move to `done/` when completed.

### 5) Scripted Helpers

PowerShell helper: `.chat/scripts/board.ps1`

Examples:

```
.\.chat\scripts\board.ps1 -Command new -Title "Add kanban board" -Owner codex
.\.chat\scripts\board.ps1 -Command move -Id ISSUE-000123 -Status doing
.\.chat\scripts\board.ps1 -Command list
.\.chat\scripts\board.ps1 -Command list -All
```

### 6) Assignments and Index

- Assignment overview: `.chat/board/ASSIGNMENTS.md`
- Index (generated): `.chat/board/index.csv`

The index is a snapshot of current issues and can be regenerated if needed.

## Current Scope Notes

- Feature specs for the component port live under `src/test/SPEC0027.ComponentLibrary/` and now cover core, form, overlay, navigation, layout, integrations/utilities, and blocks.
- High-level porting context lives in `docs/PORTING_CHADCN.md`.
- The chadcn source of truth is the local `chadcn/` clone (see that doc for paths).

## Current Build Status (2026-02-06)

Build is **green** - no compile errors.

### Pending Components (in `doing/`, awaiting implementation)
- **Accordion** (ISSUE-000009) - Components need to be created
- **Table** (ISSUE-000058) - Components need to be created
- **Tabs** (ISSUE-000059) - Components need to be created

Note: Previous compile errors mentioned in issue notes were from deleted implementations. Current state is clean - these components need to be implemented from scratch following the component library structure guidelines.

## Known Questions / Decisions

- Decision: `.cursorrules` mandates dark theme only, so `ThemeService` will be refactored to dark-only (remove `light|system`).

## Agent Loop (Execution + Supervision)

This is the required lifecycle for issue work. Every agent must follow it.

### 1) Implementer Loop (Any Agent)

1. Pick up one issue from `.chat/board/todo/` and move it to `doing/`.
2. Review the matching spec in `src/test/SPEC0027.ComponentLibrary/` and any related docs.
3. Implement the change in code.
4. Add/update tests so the spec (feature + UACs) is satisfied.
5. Run relevant tests.
6. Commit the change with a semantic message referencing the issue (example: `feat(component): add X (#123)`).
7. Move the issue to `done/` and note a short summary in the issue file (include commit hash).

### 2) Supervising Loop (codex only)

After an issue is moved to `done/`, codex must:

1. Re-open the issue and review the changes.
2. Verify spec alignment:
   - Feature file UACs match the tests.
   - Tests match the intended behavior.
3. Run targeted tests (or the best available subset).
4. If corrections are needed, move the issue back to `doing/` with notes.
5. If acceptable, add any follow-up commit(s) only if required (e.g., minor fixes from review). The primary implementation commit should already exist from the implementer.

### 3) Required Artifacts

- Spec: `.feature` file with UAC tags.
- Tests: bUnit/xUnit with full traceability traits.
- Code: component or utility implementation.
- Issue notes: summary in the `ISSUE-*.md` file.
