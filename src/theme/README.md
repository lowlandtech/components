# Theme Showcase Port Plan

This plan outlines how to port `samples/vite` into Blazor under `src/theme` using components in `src/lib/Components` as the primary showcase surface.

## Goals

- Recreate the Vite sample layout and content in Blazor pages under `src/theme`.
- Showcase existing components from `src/lib/Components` using their predefined classes and patterns.
- Keep the UI aligned with the dark-only Tailwind v4 theme and semantic color variables.

## Inventory and Mapping

The Vite sample routes cover multiple product areas (dashboards, public profile, account, network, store client/admin, auth, error). The initial port focuses on the dashboards demos (demo1-5) because they represent the core layout and component variety.

**Phase 1 route inventory (dashboards)**

- `samples/vite/src/pages/dashboards/demo1/light-sidebar`
- `samples/vite/src/pages/dashboards/demo1/dark-sidebar`
- `samples/vite/src/pages/dashboards/demo2`
- `samples/vite/src/pages/dashboards/demo3`
- `samples/vite/src/pages/dashboards/demo4`
- `samples/vite/src/pages/dashboards/demo5`

**Section mapping (demo1 light sidebar)**

| Vite section | Source | Blazor target | Notes |
| --- | --- | --- | --- |
| Channel stats cards | `samples/vite/src/pages/dashboards/demo1/light-sidebar/components/channel-stats.tsx` | `src/theme/Components/Pages/Demo1/Demo1LightSidebar.razor` | Map to card + stat patterns, no inline utility if class exists |
| Entry callout | `samples/vite/src/pages/dashboards/demo1/light-sidebar/components/entry-callout.tsx` | `src/theme/Components/Pages/Demo1/EntryCallout.razor` | Card with background image and CTA |
| Highlights | `samples/vite/src/pages/dashboards/demo1/light-sidebar/components/highlights.tsx` | `src/theme/Components/Pages/Demo1/Highlights.razor` | Card, badge, and list rows |
| Earnings chart | `samples/vite/src/pages/dashboards/demo1/light-sidebar/components/earnings-chart.tsx` | `src/theme/Components/Pages/Demo1/EarningsChart.razor` | Replace chart with placeholder or Blazor chart later |
| Team meeting | `samples/vite/src/pages/dashboards/demo1/light-sidebar/components/team-meeting.tsx` | `src/theme/Components/Pages/Demo1/TeamMeeting.razor` | Card layout with media and CTA |
| Teams table | `samples/vite/src/pages/dashboards/demo1/light-sidebar/components/teams.tsx` | `src/theme/Components/Pages/Demo1/TeamsTable.razor` | Map to table or datagrid component if available |

**Section mapping (demo2-5)**

- Demo2: options, my balance, entry callout, report settings, integrations, block list, teams, manage data.
- Demo3: create team, highlights, teams, block list, integrations, manage data.
- Demo4: create team, highlights, teams, block list.
- Demo5: team info, options, teams, FAQ.

## Folder and Page Structure

- `src/theme/Components/Pages/Demo1` for demo1 pages and partials.
- `src/theme/Components/Pages/Demo2` for demo2 pages and partials.
- `src/theme/Components/Pages/Demo3` for demo3 pages and partials.
- `src/theme/Components/Pages/Demo4` for demo4 pages and partials.
- `src/theme/Components/Pages/Demo5` for demo5 pages and partials.
- Shared showcase partials under `src/theme/Components/Shared` (entry callout, highlights, teams table, block list, etc.).

## Implementation Steps

1. Replace the default `MainLayout` shell with the theme layout shell (sidebar + header) using the peer-checked sidebar pattern if needed.
2. Create Blazor routes for the demo dashboards and wire navigation.
3. Port demo1 light sidebar sections as shared components first.
4. Assemble demo2-5 pages using the shared components and add any missing section components.
5. Normalize all styling to component classes from `src/lib/wwwroot/css/input.css`.
6. Add placeholder or simplified content where a direct Blazor equivalent is missing (charts, advanced tables).

## Coordination and Tracking (Agent Loop + Chat Board)

- Use the file-based chat/board workflow in `.chat/` as the primary coordination method.
- Create and move issue files under `.chat/board/` (`backlog` -> `todo` -> `doing` -> `done`).
- Each issue file must include spec IDs in the format `SPEC1250.USXX` or `SPEC1250.USX.SCXX`.
- Use `.chat/` message ticks for handoffs and decisions when coordinating between agents.
- Keep `issues/SPEC1250/` updated for scope context, but treat the board as the execution source of truth.

## Component Usage Rules

- Use component classes defined in `src/lib/wwwroot/css/input.css`.
- Do not recreate components with raw utility classes if a component class exists.
- Use semantic color utilities (`bg-card`, `text-foreground`, etc.).
- Keep hover/focus/transition patterns consistent with project conventions.

## Validation Checklist

- All demo pages render correctly in Blazor under `src/theme`.
- No hardcoded colors; only semantic CSS variables and component classes.
- Sidebar and layout follow the peer-checked pattern if a sidebar is used.
- Sections match the Vite sample structure and content order.
- Components showcase is comprehensive and uses real component variants.
- Visual parity checked against Vite at `http://localhost:5174` using browser snapshots.

## Follow-up

- Expand to account, network, public profile, and store-client pages after dashboards are stable.
- Add screenshots or a short usage note once the theme pages are complete.
- Consider a small bUnit test to verify key components render in the theme pages.
